@Code
    ViewBag.Title = "Home"
End Code

<div class="container" style="max-width: 1080px;">
    <!-- Hero -->
    <div class="text-center" style="padding:32px 12px;">
        <h1 class="display-4" style="margin-bottom:8px;">UCI Med School Stats</h1>
        <p class="lead" style="margin:0;">
            Explore California medical school applications &amp; matriculants, and manage feature ideas.
        </p>
    </div>

    <!-- Primary actions -->
    <div class="row" style="gap:20px 0;">
        <div class="col-md-6">
            <div class="card" style="border:1px solid #e5e5e5; border-radius:12px; box-shadow:0 2px 8px rgba(0,0,0,.04);">
                <div class="card-body" style="padding:18px 18px 8px;">
                    <h3 style="margin:0 0 8px;">Charts &amp; Insights</h3>
                    <p style="margin:0 0 12px;">
                        Interactive charts for CA medical schools:
                        <br />• Applicants by school (counts)
                        <br />• Applications % In-State vs Out-of-State
                        <br />• Matriculants by school (counts)
                        <br />• Matriculants % In-State vs Out-of-State
                    </p>
                    <a class="btn btn-primary" href="@Url.Action("Index", "Charts")">Open Charts</a>
                </div>
            </div>
        </div>

        <div class="col-md-6">
            <div class="card" style="border:1px solid #e5e5e5; border-radius:12px; box-shadow:0 2px 8px rgba(0,0,0,.04);">
                <div class="card-body" style="padding:18px 18px 8px;">
                    <h3 style="margin:0 0 8px;">Feature Requests</h3>
                    <p style="margin:0 0 12px;">
                        Log ideas, edit or delete requests, and mark items complete. Completed items are grayed out and move to the bottom.
                    </p>
                    <a class="btn btn-outline-primary" href="@Url.Action("Index", "FeatureRequests")">Manage Requests</a>
                </div>
            </div>
        </div>
    </div>

    <!-- Data status (optional: shown only if provided) -->
    <div class="row" style="margin-top:28px;">
        <div class="col-md-12">
            <div class="card" style="border:1px solid #e5e5e5; border-radius:12px;">
                <div class="card-body" style="padding:16px 18px;">
                    <h4 style="margin:0 0 8px;">Data Status</h4>

                    @If ViewBag.LastImportAt IsNot Nothing Then
                        @:<p style="margin:0 0 6px;">Last import: <strong>@(CDate(ViewBag.LastImportAt).ToLocalTime().ToString("yyyy-MM-dd HH:mm"))</strong></p>
                    End If

                    @If ViewBag.SourceFile IsNot Nothing Then
                        @:<p style="margin:0;">Source file: <code>@ViewBag.SourceFile</code></p>
                    End If

                    @If ViewBag.SchoolCount IsNot Nothing OrElse ViewBag.AppCount IsNot Nothing OrElse ViewBag.MatCount IsNot Nothing Then
                        @<text>
                            <div class="row" style="margin-top:10px;">
                                <div class="col-sm-4"><strong>Schools:</strong> @(If(ViewBag.SchoolCount, "-"))</div>
                                <div class="col-sm-4"><strong>Applications rows:</strong> @(If(ViewBag.AppCount, "-"))</div>
                                <div class="col-sm-4"><strong>Matriculants rows:</strong> @(If(ViewBag.MatCount, "-"))</div>
                            </div>
                        </text>
                    End If

                    @If ViewBag.LastImportAt Is Nothing AndAlso ViewBag.SourceFile Is Nothing AndAlso ViewBag.SchoolCount Is Nothing Then
                        @:<p class="text-muted" style="margin:0;">No data status available yet. Import data, then return here to see a summary.</p>
                    End If
                </div>
            </div>
        </div>
    </div>

    <!-- Quick links -->
    <div class="row" style="margin-top:28px;">
        <div class="col-md-12">
            <div class="card" style="border:1px solid #e5e5e5; border-radius:12px;">
                <div class="card-body" style="padding:16px 18px;">
                    <h4 style="margin:0 0 8px;">Quick Links</h4>
                    <ul style="margin:0; padding-left:18px;">
                        <li><a href="@Url.Action("Index", "Charts")">Charts</a></li>
                        <li><a href="@Url.Action("Index", "FeatureRequests")">Feature Requests</a></li>
                        <li><a href="@Url.Action("About", "Home")">About</a></li>
                        ' If you have an import endpoint, expose it here:
                        '
                        <li><a href="@Url.Action("Run", "Import")">Run Data Import</a></li>
                    </ul>
                </div>
            </div>
        </div>
    </div>
</div>
