@ModelType FeatureRequest
@Code
    ViewBag.Title = "Edit Feature Request"
End Code

<h2>@ViewBag.Title</h2>

@Using Html.BeginForm("Edit", "FeatureRequests", FormMethod.Post)
    @Html.AntiForgeryToken()
    @Html.HiddenFor(Function(m) m.Id)
    @<text>
        <div class="form-group">
            @Html.LabelFor(Function(m) m.Description)
            @Html.TextAreaFor(Function(m) m.Description, New With {.class = "form-control", .rows = "4", .required = "required"})
        </div>
        <div class="form-group">
            @Html.LabelFor(Function(m) m.RequesterName)
            @Html.TextBoxFor(Function(m) m.RequesterName, New With {.class = "form-control", .required = "required"})
        </div>
        <div class="form-group">
            @Html.LabelFor(Function(m) m.RequestedAt)
            @Html.TextBoxFor(Function(m) m.RequestedAt, "{0:yyyy-MM-ddTHH:mm}", New With {.class = "form-control", .type = "datetime-local"})
            <small class="form-text text-muted">Stored in UTC internally.</small>
        </div>
        <div class="form-group">
            @Html.LabelFor(Function(m) m.IsCompleted)
            @Html.CheckBoxFor(Function(m) m.IsCompleted)
        </div>
        <button type="submit" class="btn btn-primary">Save</button>
        @Html.ActionLink("Cancel", "Index", Nothing, New With {.class = "btn btn-default", .style = "margin-left:8px;"})
    </text> End Using
