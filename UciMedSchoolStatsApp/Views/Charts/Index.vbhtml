@Code
    ViewBag.Title = "Charts"
End Code

<h2>California Medical School Statistics</h2>

<!-- FIRST CHART -->
<h3>Applicants by School</h3>
<div style="display:flex;gap:12px;align-items:center;margin-bottom:8px;">
    <label for="firstMode"><strong>View:</strong></label>
    <select id="firstMode" aria-label="Select view for first chart">
        <option value="counts" selected>Applicants by School (Counts)</option>
        <option value="appsPct">Applications: In-State vs Out-of-State (%)</option>
    </select>
</div>
<p id="appsHint" style="display:none;color:#666;font-style:italic;margin-top:-8px;">
    Click on a school's data bar for pie chart drill-down.
</p>
<canvas id="firstChart"></canvas>

<!-- Applicants Pie (450x450) -->
<div id="appsPieContainer" style="        display: none;
        margin-top: 16px;
        width: 450px;
        height: 450px;">
    <h4 id="appsPieTitle" style="margin-bottom:8px;"></h4>
    <canvas id="appsPie"></canvas>
</div>
<br />
<!-- SECOND CHART -->
<h3 style="margin-top:40px;">Matriculants by School</h3>
<div style="display:flex;gap:12px;align-items:center;margin-bottom:8px;">
    <label for="matMode"><strong>View:</strong></label>
    <select id="matMode" aria-label="Select view for second chart">
        <option value="counts" selected>Matriculants by School (Counts)</option>
        <option value="pct">Matriculants: In-State vs Out-of-State (%)</option>
    </select>
</div>
<p id="matHint" style="display:none;color:#666;font-style:italic;margin-top:-8px;">
    Click on a school's data bar for pie chart drill-down.
</p>
<canvas id="matChart"></canvas>

<!-- Matriculants Pie (450x450) -->
<div id="matPieContainer" style="display:none;margin-top:16px;width:450px;height:450px;">
    <h4 id="matPieTitle" style="margin-bottom:8px;"></h4>
    <canvas id="matPie"></canvas>
</div>
<br />
<!-- THIRD CHART -->
<h3 style="margin-top:40px;">Applicant Matriculation Success by School</h3>
<p class="text-muted" style="margin-top:-4px;">
    Percentage of applicants who matriculated: (Matriculants ÷ Applications) × 100
</p>
<canvas id="acceptChart"></canvas>

<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.1/dist/chart.umd.min.js"></script>
<script>
(function () {
  function getJSON(url) { return fetch(url, { credentials: "same-origin" }).then(r => r.json()); }

  // ------- Palette -------
  const COLORS = {
    inStateBg: '#4E79A7', inStateBorder: '#355D84',
    outStateBg: '#F28E2B', outStateBorder: '#B5621D',
    countsBg: '#9FBFDF', countsBorder: '#7FA4C4',        // muted light blue (counts)
    acceptBg: '#F2E394', acceptBorder: '#D6C774'         // muted yellow (acceptance)
  };

  // ---------- Shared chart builders ----------
  function makeCountsConfig(labels, counts, labelText) {
    return {
      type: 'bar',
      data: { labels, datasets: [{ label: labelText, data: counts, backgroundColor: COLORS.countsBg, borderColor: COLORS.countsBorder, borderWidth: 1 }] },
      options: {
        responsive: true,
        plugins: { legend: { display: false }, tooltip: { callbacks: { label: ctx => Number(ctx.raw).toLocaleString() } } },
        scales: { x: { ticks: { autoSkip: false, maxRotation: 45 } }, y: { beginAtZero: true, ticks: { callback: v => Number(v).toLocaleString() } } }
      }
    };
  }

  function makeStackedPctConfig(labels, inPct, outPct, stackKey) {
    return {
      type: 'bar',
      data: {
        labels,
        datasets: [
          { label: 'In-State %', data: inPct, backgroundColor: COLORS.inStateBg, borderColor: COLORS.inStateBorder, borderWidth: 1, stack: stackKey },
          { label: 'Out-of-State %', data: outPct, backgroundColor: COLORS.outStateBg, borderColor: COLORS.outStateBorder, borderWidth: 1, stack: stackKey }
        ]
      },
      options: {
        responsive: true,
        plugins: { legend: { position: 'top' }, tooltip: { callbacks: { label: ctx => `${ctx.dataset.label}: ${ctx.raw}%` } } },
        scales: { x: { stacked: true, ticks: { autoSkip: false, maxRotation: 45 } }, y: { stacked: true, beginAtZero: true, max: 100, ticks: { callback: v => v + '%' } } }
      }
    };
  }

  // Only for Acceptance chart (cap at 10%)
  function makePctConfig(labels, pctValues, labelText) {
    return {
      type: 'bar',
      data: { labels, datasets: [{ label: labelText, data: pctValues, backgroundColor: COLORS.acceptBg, borderColor: COLORS.acceptBorder, borderWidth: 1 }] },
      options: {
        responsive: true,
        plugins: { legend: { display: false }, tooltip: { callbacks: { label: ctx => `${ctx.raw}%` } } },
        scales: { x: { ticks: { autoSkip: false, maxRotation: 45 } }, y: { beginAtZero: true, max: 10, ticks: { callback: v => v + '%' } } }
      }
    };
  }

  // ---------- FIRST CHART ----------
  const firstModeEl = document.getElementById('firstMode');
  const firstChartEl = document.getElementById('firstChart');
  const appsPieContainer = document.getElementById('appsPieContainer');
  const appsPieTitle = document.getElementById('appsPieTitle');
  const appsPieCanvas = document.getElementById('appsPie');
  const appsHint = document.getElementById('appsHint');

  let firstChart, cachedCounts = null, cachedAppsPct = null, appsPieChart = null;

  function renderAppsPieForIndex(idx) {
    if (!cachedAppsPct || idx < 0) return;
    const school = cachedAppsPct.labels[idx], inVal = cachedAppsPct.inPct[idx] ?? 0, outVal = cachedAppsPct.outPct[idx] ?? 0;

    if (appsPieChart) appsPieChart.destroy();
    appsPieChart = new Chart(appsPieCanvas, {
      type: 'pie',
      data: {
        labels: ['In-State %', 'Out-of-State %'],
        datasets: [{
          data: [inVal, outVal],
          backgroundColor: [COLORS.inStateBg, COLORS.outStateBg],
          borderColor: [COLORS.inStateBorder, COLORS.outStateBorder],
          borderWidth: 1
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,      // fills 350x350 container
        plugins: {
          legend: { position: 'top' },
          tooltip: { callbacks: { label: ctx => `${ctx.label}: ${ctx.raw}%` } }
        }
      }
    });

    appsPieTitle.textContent = `Applications: ${school} (In vs Out of State)`;
    appsPieContainer.style.display = 'block';
  }
  function hideAppsPie() { if (appsPieChart) { appsPieChart.destroy(); appsPieChart = null; } appsPieContainer.style.display = 'none'; }

  async function renderFirstChart(mode) {
    appsHint.style.display = (mode === 'appsPct') ? 'block' : 'none';

    if (mode === 'counts' && !cachedCounts) {
      const d = await getJSON('@Url.Action("ApplicationsData", "Charts")');
      cachedCounts = { labels: d.map(x => x.School), counts: d.map(x => x.Applications) };
    }
    if (mode === 'appsPct' && !cachedAppsPct) {
      const d = await getJSON('@Url.Action("ApplicationsPercentData", "Charts")');
      cachedAppsPct = { labels: d.map(x => x.School), inPct: d.map(x => x.InState), outPct: d.map(x => x.OutState) };
    }

    const cfg = (mode === 'counts')
      ? makeCountsConfig(cachedCounts.labels, cachedCounts.counts, 'Applications (count)')
      : makeStackedPctConfig(cachedAppsPct.labels, cachedAppsPct.inPct, cachedAppsPct.outPct, 'appsPct');

    if (firstChart) firstChart.destroy();
    firstChart = new Chart(firstChartEl, cfg);

    if (mode === 'appsPct') {
      firstChartEl.style.cursor = 'pointer';
      firstChartEl.onclick = evt => {
        const els = firstChart.getElementsAtEventForMode(evt, 'nearest', { intersect: true }, true);
        if (els.length) renderAppsPieForIndex(els[0].index);
      };
    } else {
      firstChartEl.style.cursor = 'default';
      firstChartEl.onclick = null;
      hideAppsPie();
    }
  }

  // ---------- SECOND CHART ----------
  const matModeEl = document.getElementById('matMode');
  const matChartEl = document.getElementById('matChart');
  const matPieContainer = document.getElementById('matPieContainer');
  const matPieTitle = document.getElementById('matPieTitle');
  const matPieCanvas = document.getElementById('matPie');
  const matHint = document.getElementById('matHint');

  let matChart, cachedMatCounts = null, cachedMatPct = null, matPieChart = null;

  function renderMatPieForIndex(idx) {
    if (!cachedMatPct || idx < 0) return;
    const school = cachedMatPct.labels[idx], inVal = cachedMatPct.inPct[idx] ?? 0, outVal = cachedMatPct.outPct[idx] ?? 0;

    if (matPieChart) matPieChart.destroy();
    matPieChart = new Chart(matPieCanvas, {
      type: 'pie',
      data: {
        labels: ['In-State %', 'Out-of-State %'],
        datasets: [{
          data: [inVal, outVal],
          backgroundColor: [COLORS.inStateBg, COLORS.outStateBg],
          borderColor: [COLORS.inStateBorder, COLORS.outStateBorder],
          borderWidth: 1
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,      // fills 350x350 container
        plugins: {
          legend: { position: 'top' },
          tooltip: { callbacks: { label: ctx => `${ctx.label}: ${ctx.raw}%` } }
        }
      }
    });

    matPieTitle.textContent = `Matriculants: ${school} (In vs Out of State)`;
    matPieContainer.style.display = 'block';
  }
  function hideMatPie() { if (matPieChart) { matPieChart.destroy(); matPieChart = null; } matPieContainer.style.display = 'none'; }

  async function renderMatChart(mode) {
    matHint.style.display = (mode === 'pct') ? 'block' : 'none';

    if (mode === 'counts' && !cachedMatCounts) {
      const d = await getJSON('@Url.Action("MatriculantsData", "Charts")');
      cachedMatCounts = { labels: d.map(x => x.School), counts: d.map(x => x.Matriculants) };
    }
    if (mode === 'pct' && !cachedMatPct) {
      const d = await getJSON('@Url.Action("MatriculantsPercentData", "Charts")');
      cachedMatPct = { labels: d.map(x => x.School), inPct: d.map(x => x.InState), outPct: d.map(x => x.OutState) };
    }

    const cfg = (mode === 'counts')
      ? makeCountsConfig(cachedMatCounts.labels, cachedMatCounts.counts, 'Matriculants (count)')
      : makeStackedPctConfig(cachedMatPct.labels, cachedMatPct.inPct, cachedMatPct.outPct, 'matPct');

    if (matChart) matChart.destroy();
    matChart = new Chart(matChartEl, cfg);

    if (mode === 'pct') {
      matChartEl.style.cursor = 'pointer';
      matChartEl.onclick = evt => {
        const els = matChart.getElementsAtEventForMode(evt, 'nearest', { intersect: true }, true);
        if (els.length) renderMatPieForIndex(els[0].index);
      };
    } else {
      matChartEl.style.cursor = 'default';
      matChartEl.onclick = null;
      hideMatPie();
    }
  }

  // ---------- THIRD CHART (Acceptance % with 10% cap) ----------
  const acceptChartEl = document.getElementById('acceptChart');
  let acceptChart, cachedAccept = null;

  async function renderAcceptChart() {
    if (!cachedAccept) {
      const d = await getJSON('@Url.Action("MatriculationSuccess", "Charts")');
      cachedAccept = {
        labels: d.map(x => x.School),
        pct: d.map(x => x.AcceptancePct),
        apps: d.map(x => x.Applications),
        mats: d.map(x => x.Matriculants)
      };
    }

    const cfg = makePctConfig(cachedAccept.labels, cachedAccept.pct, 'Acceptance Rate (%)');
    cfg.options.plugins.tooltip.callbacks.afterLabel = ctx => {
      const i = ctx.dataIndex;
      return `\nApplications: ${cachedAccept.apps[i].toLocaleString()}\nMatriculants: ${cachedAccept.mats[i].toLocaleString()}`;
    };

    if (acceptChart) acceptChart.destroy();
    acceptChart = new Chart(acceptChartEl, cfg);
  }

  // Init
  renderFirstChart('counts');
  renderMatChart('counts');
  renderAcceptChart();

  // Toggles
  firstModeEl.addEventListener('change', e => renderFirstChart(e.target.value));
  matModeEl.addEventListener('change', e => renderMatChart(e.target.value));
})();
</script>
