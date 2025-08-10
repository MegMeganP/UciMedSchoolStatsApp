Imports System.Web.Mvc
Imports System.Data.Entity

Public Class FeatureRequestsController
    Inherits Controller

    ' GET: /FeatureRequests
    Function Index() As ActionResult
        Using ctx As New DataContext()
            ' Order: incomplete first (newest first), then completed (newest first),
            ' The view will gray completed rows; ordering ensures completed appear at bottom.
            Dim items = ctx.FeatureRequests _
                .OrderBy(Function(r) r.IsCompleted) _
                .ThenByDescending(Function(r) r.RequestedAt) _
                .ToList()
            Return View(items)
        End Using
    End Function

    ' POST: /FeatureRequests/Create
    <HttpPost, ValidateAntiForgeryToken>
    Function Create(description As String, requesterName As String) As ActionResult
        If String.IsNullOrWhiteSpace(description) OrElse String.IsNullOrWhiteSpace(requesterName) Then
            TempData("Error") = "Please provide both a description and a name."
            Return RedirectToAction("Index")
        End If
        Using ctx As New DataContext()
            Dim fr = New FeatureRequest With {
                .Description = description.Trim(),
                .RequesterName = requesterName.Trim(),
                .RequestedAt = DateTime.UtcNow,
                .IsCompleted = False
            }
            ctx.FeatureRequests.Add(fr)
            ctx.SaveChanges()
        End Using
        Return RedirectToAction("Index")
    End Function

    ' GET: /FeatureRequests/Edit/5
    Function Edit(id As Integer?) As ActionResult
        If Not id.HasValue Then Return HttpNotFound()
        Using ctx As New DataContext()
            Dim fr = ctx.FeatureRequests.Find(id.Value)
            If fr Is Nothing Then Return HttpNotFound()
            Return View(fr)
        End Using
    End Function

    ' POST: /FeatureRequests/Edit/5
    <HttpPost, ValidateAntiForgeryToken>
    Function Edit(model As FeatureRequest) As ActionResult
        If Not ModelState.IsValid Then Return View(model)
        Using ctx As New DataContext()
            ctx.Entry(model).State = EntityState.Modified
            ' keep RequestedAt & IsCompleted as-is unless changed in form; you can restrict binding if desired
            ctx.SaveChanges()
        End Using
        Return RedirectToAction("Index")
    End Function

    ' POST: /FeatureRequests/Delete/5
    <HttpPost, ValidateAntiForgeryToken>
    Function Delete(id As Integer) As ActionResult
        Using ctx As New DataContext()
            Dim fr = ctx.FeatureRequests.Find(id)
            If fr IsNot Nothing Then
                ctx.FeatureRequests.Remove(fr)
                ctx.SaveChanges()
            End If
        End Using
        Return RedirectToAction("Index")
    End Function

    ' POST: /FeatureRequests/ToggleComplete/5
    <HttpPost, ValidateAntiForgeryToken>
    Function ToggleComplete(id As Integer) As ActionResult
        Using ctx As New DataContext()
            Dim fr = ctx.FeatureRequests.Find(id)
            If fr Is Nothing Then Return RedirectToAction("Index")
            fr.IsCompleted = Not fr.IsCompleted
            ctx.SaveChanges()
        End Using
        ' Full reload re-applies ordering so row jumps to/from bottom
        Return RedirectToAction("Index")
    End Function
End Class
