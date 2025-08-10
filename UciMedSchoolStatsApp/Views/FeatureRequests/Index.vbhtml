@ModelType IEnumerable(Of FeatureRequest)
@Code
    ViewBag.Title = "Feature Requests"
End Code

<h2>@ViewBag.Title</h2>

@If TempData("Error") IsNot Nothing Then
    @:<div class="alert alert-danger">@TempData("Error")</div>
End If

<div class="row" style="margin-top:10px;">
    <div class="col-md-6">
        <h3>Add a Request</h3>
        @Using Html.BeginForm("Create", "FeatureRequests", FormMethod.Post, New With {.class = "form"})
            @Html.AntiForgeryToken()
            @<text>
                <div class="form-group">
                    <label for="description">Request Description</label>
                    <textarea id="description" name="description" class="form-control" rows="3" required></textarea>
                </div>
                <div class="form-group">
                    <label for="requesterName">Name</label>
                    <input id="requesterName" name="requesterName" class="form-control" required />
                </div>
                <button type="submit" class="btn btn-primary">Submit</button>
            </text>
        End Using
    </div>
</div>

<hr />

<h3>Requests</h3>
<table class="table table-hover">
    <thead>
        <tr>
            <th style="width:190px;">Date/Time</th>
            <th>Description</th>
            <th style="width:180px;">Requester</th>
            <th style="width:200px;">Actions</th>
            <th style="width:140px;">Completed</th>
        </tr>
    </thead>
    <tbody>
        @For Each r In Model
            @<text>
                <tr class="@(If(r.IsCompleted, "completed-row", ""))">
                    <td>@r.RequestedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm")</td>
                    <td>@r.Description</td>
                    <td>@r.RequesterName</td>
                    <td>
                        @Html.ActionLink("Edit", "Edit", New With {.id = r.Id}, New With {.class = "btn btn-sm btn-warning"})
                        @Using Html.BeginForm("Delete", "FeatureRequests", FormMethod.Post, New With {.style = "display:inline-block;margin-left:6px;"})
                            @Html.AntiForgeryToken()
                            @<text>
                                <input type="hidden" name="id" value="@r.Id" />
                                <button type="submit" class="btn btn-sm btn-danger" onclick="return confirm('Delete this request?');">
                                    Delete
                                </button>
                            </text>
                        End Using
                    </td>
                    <td class="text-center">
                        @Using Html.BeginForm("ToggleComplete", "FeatureRequests", FormMethod.Post, New With {.style = "display:inline"})
                            @Html.AntiForgeryToken()
                            @<text>
                                <input type="hidden" name="id" value="@r.Id" />
                                <input type="checkbox" name="isCompleted" value="true" onchange="this.form.submit()" @(If(r.IsCompleted, "checked=""checked""", "")) />
                            </text>
                        End Using
                    </td>
                </tr>
            </text>
        Next
    </tbody>
</table>

<style>
    .completed-row {
        color: #666;
        opacity: .75;
    }

        .completed-row td {
            background: #f2f2f2 !important;
        }
</style>
