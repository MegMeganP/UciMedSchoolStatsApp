@Code
    ViewBag.Title = "Home"
End Code

<style>
    :root{
        --blue-border:#7FA4C4;
        --blue-shadow:0 6px 18px rgba(127,164,196,.25);
        --blue-accent:#4E79A7;
        --blue-accent-hover:#3F658C;
        --page-bg:#F6FAFF;
        --text-muted:#6b7280;
        --card-radius:14px;
    }

    /* Full-width page background (kills side white gutters) */
    html, body { height:100%; }
    body{
        background: linear-gradient(180deg, var(--page-bg), #ffffff);
        margin:0;
    }

    .page-wrap{
        padding: 24px 12px 36px;
    }

    /* Header */
    .page-title{
        font-size: 3.5rem;            /* bigger title */
        line-height: 1.15;
        font-weight: 800;
        text-align: center;
        margin-bottom: 10px;
        color: #0f172a;
        letter-spacing: .2px;
        position: relative;
    }
    @@media (min-width: 992px){
        .page-title{ font-size: 4rem; } /* even bigger on desktop */
    }
    .page-title::after{
        content: "";
        display: block;
        width: 110px;
        height: 5px;
        background: var(--blue-accent);
        margin: 14px auto 0;
        border-radius: 3px;
    }
    .page-subtitle{
        text-align: center;
        font-size: 2rem;
        color: var(--text-muted);
        margin-bottom: 28px;
    }

    /* Equal height row */
    .row-balanced{
        display: flex;
        flex-wrap: wrap;
        gap: 24px;
    }
    .col-half{
        flex: 1 1 420px;
        min-width: 320px;
        display: flex;
    }

    /* Cards */
    .card-ui{
        border: 1px solid var(--blue-border);
        border-radius: var(--card-radius);
        box-shadow: var(--blue-shadow);
        background: #fff;
        display: flex;
        flex-direction: column;
        flex: 1;
        overflow: hidden; /* keeps content clipped to rounded corners */
    }
    .card-img-wrap{
        height: 200px;                 /* a touch taller for presence */
        overflow: hidden;
        border-bottom: 1px solid #e8eef6;
        background: #f3f7fc;
    }
    .card-img-wrap img{
        width: 100%;
        height: 100%;
        object-fit: cover;
        display: block;
        /* round top corners to match the card */
        border-top-left-radius: var(--card-radius);
        border-top-right-radius: var(--card-radius);
    }
    .card-body{
        padding: 18px;
        display: flex;
        flex-direction: column;
        flex: 1;
    }

    /* Buttons (uniform) */
    .btn-main{
        background: var(--blue-accent);
        color: #fff;
        border: none;
        padding: .6rem 1.05rem;
        border-radius: 8px;
        text-decoration: none;
        display: inline-block;
        transition: background .2s ease, transform .05s ease;
    }
    .btn-main:hover{
        background: var(--blue-accent-hover);
        color: #fff;
    }
    .btn-main:active{ transform: translateY(1px); }
    .btn-row{
        display: flex;
        gap: 10px;
        flex-wrap: wrap;
        margin-top: auto; /* pushes buttons to the bottom so both cards equalize */
    }

    /* Quick links (subtle list, no box) */
    .quick-links{
        text-align: center;
        margin-top: 36px;
    }
    .quick-links ul{
        list-style: none;
        padding: 0;
        margin: 0;
        display: inline-flex;
        gap: 20px;
        flex-wrap: wrap;
    }
    .quick-links a{
        text-decoration: none;
        color: var(--blue-accent);
        font-weight: 500;
        padding-bottom: 2px;
        border-bottom: 1px solid transparent;
        transition: color .2s ease, border-color .2s ease;
    }
    .quick-links a:hover{
        color: var(--blue-accent-hover);
        border-bottom-color: var(--blue-border);
    }
</style>

<div class="page-wrap">
    <div class="container" style="max-width: 1080px;">

        <!-- Title -->
        <h1 class="page-title">UCI Medical School Statistics</h1>
        <p class="page-subtitle">
            Explore California medical school application data and manage feature requests in this interactive web app.
            <br>
            <small>*Note: This is a test app and not directly affiliated with UCI Medical School</small>
        </p>

        <!-- Primary actions -->
        <div class="row-balanced">
            <!-- Charts -->
            <div class="col-half">
                <div class="card-ui">
                    <div class="card-img-wrap">
                        <img src="~/Images/UciCharts.png" alt="Charts and Data Insights">
                    </div>
                    <div class="card-body">
                        <h3>Charts &amp; Data Insights</h3>
                        <p>
                            Interactive charts for CA medical schools:
                            <br>• Applicants by school (counts)
                            <br>• Applications % In-State vs Out-of-State
                            <br>• Matriculants by school (counts)
                            <br>• Matriculants % In-State vs Out-of-State
                            <br>• Applicant Matriculation Success Rates
                        </p>
                        <div class="btn-row">
                            <a class="btn-main" href="@Url.Action("Index", "Charts")">Open Charts</a>
                            <a class="btn-main" href="https://medschool.uci.edu/">Go To UCI</a>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Feature Requests -->
            <div class="col-half">
                <div class="card-ui">
                    <div class="card-img-wrap">
                        <img src="~/Images/UciRequesting.png" alt="Feature Requests">
                    </div>
                    <div class="card-body">
                        <h3>Feature Requests</h3>
                        <p>
                            Share site ideas, edit or delete requests, and mark items complete – submit and manage feature requests all in one place.
                        </p>
                        <div class="btn-row">
                            <a class="btn-main" href="@Url.Action("Index", "FeatureRequests")">Request Feature</a>
                            <a class="btn-main" href="@Url.Action("Index", "FeatureRequests")">Manage Requests</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        
    </div>
</div>
