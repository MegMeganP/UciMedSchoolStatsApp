<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>@ViewBag.Title - My ASP.NET Application</title>
    @Styles.Render("~/Content/css")
    @Scripts.Render("~/bundles/modernizr")
<style>
    /* Center + size footer logo image */
    .footer-logo {
        display: block;
        margin: 0 auto; /* centers horizontally */
        height: 50px; /* smaller size */
        padding-top: 10px;
        padding-bottom: 10px;
    }


    .navbar .navbar-brand,
    .navbar .navbar-nav > li > a {
        color: #cce3f7 !important; /* light pastel blue */
    }

    .navbar .navbar-brand:hover,
    .navbar .navbar-nav > li > a:hover,
    .navbar .navbar-nav > li > a:focus {
        color: #FFFFFF !important; /* even lighter on hover */
    }

    footer {
        background-color: #f8f9fa; /* subtle light background */
        padding: 10px 10px;
        text-align: center;
    }

    footer .quick-links-list {
        list-style: none !important;
        margin: 0 !important;
        padding: 0 !important;
    }

    footer .quick-links-list > li {
        display: inline-block;
        margin: 0 5px;
    }

    footer .quick-links-list > li > a {
        color: #4E79A7;
        text-decoration: none;
        font-size: 1.25rem;
        border-bottom: 1px solid transparent;
        transition: color .2s ease, border-color .2s ease;
    }

    footer .quick-links-list > li > a:hover {
        color: #3F658C;
        border-bottom-color: #7FA4C4;
    }



</style>

</head>
<body>
    <div class="navbar navbar-inverse navbar-fixed-top">
        <div class="container">
            <div class="navbar-header">
                <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </button>
                @Html.ActionLink("UCI Medical School Statistics", "Index", "Home", New With {.area = ""}, New With {.class = "navbar-brand"})
            </div>
            <div class="navbar-collapse collapse">
                <ul class="nav navbar-nav">
                    <li>@Html.ActionLink("Home", "Index", "Home")</li>
                    <li>@Html.ActionLink("Charts", "Index", "Charts")</li>
                    <li>@Html.ActionLink("About", "About", "Home")</li>
                    <li>@Html.ActionLink("Requests", "Index", "FeatureRequests")</li>
                </ul>
            </div>
        </div>
    </div>
    <div class="container body-content">
        @RenderBody()
        <hr />
    </div>

    <footer>
        <div class="quick-links">
            <ul class="list-inline quick-links-list">
                <li><a href="@Url.Action("Index", "Charts")">Charts</a></li>
                <li><a href="@Url.Action("Index", "FeatureRequests")">Feature Requests</a></li>
                <li><a href="@Url.Action("About", "Home")">About</a></li>
                <li><a href="https://medschool.uci.edu/">UCI Medical School</a></li>
                <li><a href="@Url.Action("Data", "Import")">Run Data Import</a></li>
            </ul>
        </div>
        <a href="https://medschool.uci.edu/">
            <img src="~/Images/UciLogo.png" alt="UCI Med School" class="footer-logo" />
        </a>
        <p>&copy; @DateTime.Now.Year - Megan's ASP.NET VB Test Application</p>
    </footer>


    @Scripts.Render("~/bundles/jquery")
    @Scripts.Render("~/bundles/bootstrap")
    @RenderSection("scripts", required:=False)
</body>
</html>
