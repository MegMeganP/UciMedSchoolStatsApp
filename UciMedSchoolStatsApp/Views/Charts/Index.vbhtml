@Code
    ViewBag.Title = "Charts"
End Code

<h2>Medical School Stats (CA)</h2>

<!-- FIRST CHART -->
<h3>Applicants (First Chart)</h3>
<div style="display:flex;gap:12px;align-items:center;margin-bottom:8px;">
    <label for="firstMode"><strong>View:</strong></label>
    <select id="firstMode" aria-label="Select view for first chart">
        <option value="counts" selected>Applicants by School (Counts)</option>
        <option value="appsPct">Applications: In-State vs Out-of-State (%)</option>
    </select>
</div>
<canvas id="firstChart"></canvas>
<!-- Pie container for first chart -->
<div id="appsPieContainer" style="display:none;margin-top:16px;">
    <h4 id="appsPieTitle" style="margin-bottom:8px;"></h4>
    <canvas id="appsPie" width="420" height="300"></canvas>
</div>

<!-- SECOND CHART -->
<h3 style="margin-top:40px;">Matriculants (Second Chart)</h3>
<div style="display:flex;gap:12px;align-items:center;margin-bottom:8px;">
    <label for="matMode"><strong>View:</strong></label>
    <select id="matMode" aria-label="Select view for second chart">
        <option value="counts" selected>Matriculants by School (Counts)</option>
        <option value="pct">Matriculants: In-State vs Out-of-State (%)</option>
    </select>
</div>
<canvas id="matChart"></canvas>
<!-- Pie container for second chart -->
<div id="matPieContainer" style="display:none;margin-top:16px;">
    <h4 id="matPieTitle" style="margin-bottom:8px;"></h4>
    <canvas id="matPie" width="420" height="300"></canvas>
</div>

<!-- Chart.js -->
<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.1/dist/chart.umd.min.js"></script>

<script>
(function () {
  function getJSON(url) { return fetch(url, { credentials: "same-origin" }).then(r => r.json()); }
  function formatPct(v) { return (v != null ? v : 0) + '%'; }

  // ---------------------------
  // FIRST CHART (Applicants)
  // ---------------------------
  const firstModeEl = document.getElementById('firstMode');
  const firstChartEl = document.getElementById('firstChart');

  const appsPieContainer = document.getElementById('appsPieContainer');
  const appsPieTitle = document.getElementById('appsPieTitle');
  const appsPieCanvas = document.getElementById('appsPie');

  let firstChart;
  let cachedCounts = null;   // { labels:[], counts:[] }
  let cachedAppsPct = null;  // { labels:[], inPct:[], outPct:[] }
  let appsPieChart = null;

  function makeCountsConfig(labels, counts, labelText) {
    return {
      type: 'bar',
      data: { labels, datasets: [{ label: labelText, data: counts }] },
      options: {
        responsive: true,
        plugins: {
          legend: { display: false },
          tooltip: { callbacks: { label: ctx => Number(ctx.raw).toLocaleString() } }
        },
        scales: {
          x: { ticks: { autoSkip: false, maxRotation: 45 } },
          y: { beginAtZero: true, ticks: { callback: v => Number(v).toLocaleString() } }
        }
      }
    };
  }

  function makeStackedPctConfig(labels, inPct, outPct, stackKey) {
    return {
      type: 'bar',
      data: {
        labels,
        datasets: [
          { label: 'In-State %', data: inPct, stack: stackKey },
          { label: 'Out-of-State %', data: outPct, stack: stackKey }
        ]
      },
      options: {
        responsive: true,
        plugins: {
          legend: { position: 'top' },
          tooltip: { callbacks: { label: ctx => `${ctx.dataset.label}: ${ctx.raw}%` } }
        },
        scales: {
          x: { stacked: true, ticks: { autoSkip: false, maxRotation: 45 } },
          y: { stacked: true, beginAtZero: true, max: 100, ticks: { callback: v => v + '%' } }
        }
      }
    };
  }

  function renderAppsPieForIndex(idx) {
    if (!cachedAppsPct || idx == null || idx < 0) return;
    const school = cachedAppsPct.labels[idx];
    const inVal = cachedAppsPct.inPct[idx] ?? 0;
    const outVal = cachedAppsPct.outPct[idx] ?? 0;

    const data = {
      labels: ['In-State %', 'Out-of-State %'],
      datasets: [{ data: [inVal, outVal] }]
    };
    const options = {
      responsive: true,
      plugins: {
        legend: { position: 'top' },
        tooltip: { callbacks: { label: ctx => `${ctx.label}: ${ctx.raw}%` } }
      }
    };

    if (appsPieChart) appsPieChart.destroy();
    appsPieChart = new Chart(appsPieCanvas, { type: 'pie', data, options });
    appsPieTitle.textContent = `Applications: ${school} (In vs Out of State)`;
    appsPieContainer.style.display = 'block';
  }

  function hideAppsPie() {
    if (appsPieChart) { appsPieChart.destroy(); appsPieChart = null; }
    appsPieContainer.style.display = 'none';
  }

  async function renderFirstChart(mode) {
    // load caches
    if (mode === 'counts' && !cachedCounts) {
      const d = await getJSON('@Url.Action("ApplicationsData", "Charts")');
      cachedCounts = { labels: d.map(x => x.School), counts: d.map(x => x.Applications) };
    }
    if (mode === 'appsPct' && !cachedAppsPct) {
      const d = await getJSON('@Url.Action("ApplicationsPercentData", "Charts")');
      cachedAppsPct = { labels: d.map(x => x.School), inPct: d.map(x => x.InState), outPct: d.map(x => x.OutState) };
    }

    // build config
    let cfg = (mode === 'counts')
      ? makeCountsConfig(cachedCounts.labels, cachedCounts.counts, 'Applications (count)')
      : makeStackedPctConfig(cachedAppsPct.labels, cachedAppsPct.inPct, cachedAppsPct.outPct, 'appsPct');

    // (re)render chart
    if (firstChart) firstChart.destroy();
    firstChart = new Chart(firstChartEl, cfg);

    // click-to-pie only in % mode
    if (mode === 'appsPct') {
      firstChartEl.style.cursor = 'pointer';
      firstChartEl.onclick = (evt) => {
        const els = firstChart.getElementsAtEventForMode(evt, 'nearest', { intersect: true }, true);
        if (!els || els.length === 0) return;
        const idx = els[0].index; // bar index (school)
        renderAppsPieForIndex(idx);
      };
    } else {
      firstChartEl.style.cursor = 'default';
      firstChartEl.onclick = null;
      hideAppsPie();
    }
  }

  // ---------------------------
  // SECOND CHART (Matriculants)
  // ---------------------------
  const matModeEl = document.getElementById('matMode');
  const matChartEl = document.getElementById('matChart');

  const matPieContainer = document.getElementById('matPieContainer');
  const matPieTitle = document.getElementById('matPieTitle');
  const matPieCanvas = document.getElementById('matPie');

  let matChart;
  let cachedMatCounts = null; // { labels:[], counts:[] }
  let cachedMatPct = null;    // { labels:[], inPct:[], outPct:[] }
  let matPieChart = null;

  function renderMatPieForIndex(idx) {
    if (!cachedMatPct || idx == null || idx < 0) return;
    const school = cachedMatPct.labels[idx];
    const inVal = cachedMatPct.inPct[idx] ?? 0;
    const outVal = cachedMatPct.outPct[idx] ?? 0;

    const data = {
      labels: ['In-State %', 'Out-of-State %'],
      datasets: [{ data: [inVal, outVal] }]
    };
    const options = {
      responsive: true,
      plugins: {
        legend: { position: 'top' },
        tooltip: { callbacks: { label: ctx => `${ctx.label}: ${ctx.raw}%` } }
      }
    };

    if (matPieChart) matPieChart.destroy();
    matPieChart = new Chart(matPieCanvas, { type: 'pie', data, options });
    matPieTitle.textContent = `Matriculants: ${school} (In vs Out of State)`;
    matPieContainer.style.display = 'block';
  }

  function hideMatPie() {
    if (matPieChart) { matPieChart.destroy(); matPieChart = null; }
    matPieContainer.style.display = 'none';
  }

  async function renderMatChart(mode) {
    if (mode === 'counts' && !cachedMatCounts) {
      const d = await getJSON('@Url.Action("MatriculantsData", "Charts")');
      cachedMatCounts = { labels: d.map(x => x.School), counts: d.map(x => x.Matriculants) };
    }
    if (mode === 'pct' && !cachedMatPct) {
      const d = await getJSON('@Url.Action("MatriculantsPercentData", "Charts")');
      cachedMatPct = { labels: d.map(x => x.School), inPct: d.map(x => x.InState), outPct: d.map(x => x.OutState) };
    }

    let cfg = (mode === 'counts')
      ? makeCountsConfig(cachedMatCounts.labels, cachedMatCounts.counts, 'Matriculants (count)')
      : makeStackedPctConfig(cachedMatPct.labels, cachedMatPct.inPct, cachedMatPct.outPct, 'matPct');

    if (matChart) matChart.destroy();
    matChart = new Chart(matChartEl, cfg);

    if (mode === 'pct') {
      matChartEl.style.cursor = 'pointer';
      matChartEl.onclick = (evt) => {
        const els = matChart.getElementsAtEventForMode(evt, 'nearest', { intersect: true }, true);
        if (!els || els.length === 0) return;
        const idx = els[0].index;
        renderMatPieForIndex(idx);
      };
    } else {
      matChartEl.style.cursor = 'default';
      matChartEl.onclick = null;
      hideMatPie();
    }
  }

  // init both charts
  renderFirstChart('counts');
  renderMatChart('counts');

  // toggles
  firstModeEl.addEventListener('change', e => renderFirstChart(e.target.value));
  matModeEl.addEventListener('change', e => renderMatChart(e.target.value));
})();
</script>
