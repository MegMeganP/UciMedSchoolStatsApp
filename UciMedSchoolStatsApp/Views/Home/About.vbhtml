@Code
    ViewBag.Title = "About"
End Code

<div class="container" style="max-width:900px;">
    <h2>@ViewBag.Title</h2>

    <!-- About Me -->
    <section style="margin-top:16px;">
        <h3>About Me</h3>
        <p>
            Hi, I’m Megan. I enjoy building apps and exploring data.  I learn new skills quickly and love working as a member of a diverse team.  I am ready to contribute to your projects with my application development skills!
        </p>
    </section>

    <!-- About the App -->
    <section style="margin-top:28px;">
        <h3>About This App</h3>
        <p>
            UCI Medical School Stats Application is an ASP.NET MVC (VB / .NET Framework 4.8) web application that downloads an AAMC
            data file, parses California medical schools, and loads the results into SQL Server using
            Entity Framework Code-First Migrations. It then visualizes the data with interactive JSCharts.
        </p>

        <h4 style="margin-top:16px;">Building the UCI Medical School Stats Application</h4>
        <ul>
            <li>Created a new ASP.NET MVC app in Visual Studio 2019 (VB, .NET Framework 4.8).  MVC VB Template is not supported in VS 2022.</li>
            <li>Added a service to download and read the AAMC file (CSV/XLSX) and import California rows.</li>
            <li>Modeled three tables with EF6: <em>CaMedSchool</em>, <em>Applications</em>, and <em>Matriculants</em>.</li>
            <li>Used EF6 Code-First Migrations to create/update a SQL Server database.</li>
            <li>Added controllers for necessary crud operations.</li>
            <li>Built charts with Chart.js: counts and in-state vs out-of-state comparisons (stacked bars).</li>
            <li>Added click actions on % charts to show per-school pie charts.</li>
            <li>Added Feature Request functionality with a <em>FeatureRequest</em> table in SQL database, model, controller, and views.</li>
            <li>Updated the styling with CSS/Bootstrap to improve visual appeal.</li>
            <li>Deployed the application with Azure through Visual Studio.</li>
        </ul>

        <h4 style="margin-top:16px;">Resources I used</h4>
        <ul>
            <li>Entity Framework 6 (Code-First &amp; Migrations)</li>
            <li>ExcelDataReader / ExcelDataReader.DataSet (for reading .xlsx)</li>
            <li>Chart.js (client-side charting library)</li>
            <li>SQL Server LocalDB with SSMS for verification</li>
            <li>ASP.NET MVC 5 (VB) project template in Visual Studio 2019</li>
            <li>AAMC data file (Applications &amp; Matriculants)</li>
        </ul>

        <h4 style="margin-top:16px;">Resource Links</h4>
        <ul style="margin-top:8px;">
            <li><a href="https://www.nuget.org/packages/EntityFramework" target="_blank" rel="noopener">Entity Framework (NuGet)</a></li>
            <li><a href="https://www.nuget.org/packages/ExcelDataReader" target="_blank" rel="noopener">ExcelDataReader (NuGet)</a></li>
            <li><a href="https://www.chartjs.org/" target="_blank" rel="noopener">Chart.js</a></li>
            <li><a href="https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-aspnet-mvc3/vb/" target="_blank" rel="noopener">ASP.NET MVC VB.NET (docs)</a></li>
            <li><a href="https://www.aamc.org/data-reports/students-residents/data/facts-applicants-and-matriculants" target="_blank" rel="noopener">AAMC Data Reports</a></li>
            <li><a href="https://medschool.uci.edu/" target="_blank" rel="noopener">UC Irvine School of Medicine</a></li>

        </ul>
    </section>
</div>

