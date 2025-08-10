@Code
    ViewBag.Title = "Charts"
End Code

<h2>Medical School Stats (CA)</h2>

<!-- FIRST CHART: toggle counts vs applications % -->
<h3>Applicants (First Chart)</h3>
<div style="display:flex;gap:12px;align-items:center;margin-bottom:8px;">
    <label for="firstMode"><strong>View:</strong></label>
    <select id="firstMode" aria-label="Select view for first chart">
        <option value="counts" selected>Applicants by School (Counts)</option>
        <option value="appsPct">Applications: In-State vs Out-of-State (%)</option>
    </select>
</div>
<canvas id="firstChart"></canvas>

<!-- SECOND CHART: toggle counts vs matriculants % -->
<h3 style="margin-top:40px;">Matriculants (Second Chart)</h3>
<div style="display:flex;gap:12px;align-items:center;margin-bottom:8px;">
    <label for="matMode"><strong>View:</strong></label>
    <select id="matMode" aria-label="Select view for second chart">
        <option value="counts" selected>Matriculants by School (Counts)</option>
        <option value="pct">Matriculants: In-State vs Out-of-State (%)</option>
    </select>
</div>
<canvas id="matChart"></canvas>

<!-- Chart.js -->
<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.1/dist/chart.umd.min.js"></script>

<script>
(function () {
  function getJSON(url) { return fetch(url, { credentials: "same-origin" }).then(r => r.json()); }

  // ---------------------------
  // FIRST CHART (Applicants)
  // ---------------------------
  const firstModeEl = document.getElementById('firstMode');
  const firstChartEl = document.getElementById('firstChart');
  let firstChart;
  let cachedCounts = null;   // { labels:[], counts:[] }
  let cachedAppsPct = null;  // { labels:[], inPct:[], outPct:[] }

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

  async function renderFirstChart(mode) {
    if (mode === 'counts' && !cachedCounts) {
      const d = await getJSON('@Url.Action("ApplicationsData", "Charts")');
      cachedCounts = { labels: d.map(x => x.School), counts: d.map(x => x.Applications) };
    }
    if (mode === 'appsPct' && !cachedAppsPct) {
      const d = await getJSON('@Url.Action("ApplicationsPercentData", "Charts")');
      cachedAppsPct = { labels: d.map(x => x.School), inPct: d.map(x => x.InState), outPct: d.map(x => x.OutState) };
    }

    let cfg;
    if (mode === 'counts') {
      cfg = makeCountsConfig(cachedCounts.labels, cachedCounts.counts, 'Applications (count)');
    } else {
      cfg = makeStackedPctConfig(cachedAppsPct.labels, cachedAppsPct.inPct, cachedAppsPct.outPct, 'appsPct');
    }

    if (firstChart) firstChart.destroy();
    firstChart = new Chart(firstChartEl, cfg);
  }

  // ---------------------------
  // SECOND CHART (Matriculants)
  // ---------------------------
  const matModeEl = document.getElementById('matMode');
  const matChartEl = document.getElementById('matChart');
  let matChart;
  let cachedMatCounts = null; // { labels:[], counts:[] }
  let cachedMatPct = null;    // { labels:[], inPct:[], outPct:[] }

  async function renderMatChart(mode) {
    if (mode === 'counts' && !cachedMatCounts) {
      const d = await getJSON('@Url.Action("MatriculantsData", "Charts")');
      cachedMatCounts = { labels: d.map(x => x.School), counts: d.map(x => x.Matriculants) };
    }
    if (mode === 'pct' && !cachedMatPct) {
      const d = await getJSON('@Url.Action("MatriculantsPercentData", "Charts")');
      cachedMatPct = { labels: d.map(x => x.School), inPct: d.map(x => x.InState), outPct: d.map(x => x.OutState) };
    }

    let cfg;
    if (mode === 'counts') {
      cfg = makeCountsConfig(cachedMatCounts.labels, cachedMatCounts.counts, 'Matriculants (count)');
    } else {
      cfg = makeStackedPctConfig(cachedMatPct.labels, cachedMatPct.inPct, cachedMatPct.outPct, 'matPct');
    }

    if (matChart) matChart.destroy();
    matChart = new Chart(matChartEl, cfg);
  }

  // init both charts
  renderFirstChart('counts');
  renderMatChart('counts');

  // toggles
  firstModeEl.addEventListener('change', e => renderFirstChart(e.target.value));
  matModeEl.addEventListener('change', e => renderMatChart(e.target.value));
})();
</script>
