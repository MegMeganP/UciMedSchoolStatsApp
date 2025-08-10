Imports System.Web.Mvc
Imports System.Linq

Public Class ChartsController
    Inherits Controller

    ' View with the two charts
    Function Index() As ActionResult
        Return View()
    End Function

    ' 1) Applications COUNT by school (for the first chart bars)
    Function ApplicationsData() As ActionResult
        Using ctx As New DataContext()
            Dim q = (From s In ctx.Schools
                     Join a In ctx.Applications On s.ID Equals a.SchoolID
                     Order By s.Name
                     Select New With {
                         .School = s.Name,
                         .Applications = a.NumberApplications
                     }).ToList()

            Return Json(q, JsonRequestBehavior.AllowGet)
        End Using
    End Function

    ' 2) Applications % (in/out) for overlay
    Function ApplicationsPercentData() As ActionResult
        Using ctx As New DataContext()
            Dim q = (From s In ctx.Schools
                     Join a In ctx.Applications On s.ID Equals a.SchoolID
                     Order By s.Name
                     Select New With {
                         .School = s.Name,
                         .InState = a.InStateAppPercent,
                         .OutState = a.OutOfStateAppPercent
                     }).ToList()

            Return Json(q, JsonRequestBehavior.AllowGet)
        End Using
    End Function

    ' 3) Matriculants % (in/out) for overlay and for the second chart
    Function MatriculantsPercentData() As ActionResult
        Using ctx As New DataContext()
            Dim q = (From s In ctx.Schools
                     Join m In ctx.Matriculants On s.ID Equals m.SchoolID
                     Order By s.Name
                     Select New With {
                         .School = s.Name,
                         .InState = m.InStateMatPercent,
                         .OutState = m.OutOfStateMatPercent
                     }).ToList()

            Return Json(q, JsonRequestBehavior.AllowGet)
        End Using
    End Function

    ' Matriculants COUNT by school (for the "counts" mode on the second chart)
    Function MatriculantsData() As ActionResult
        Using ctx As New DataContext()
            Dim q = (From s In ctx.Schools
                     Join m In ctx.Matriculants On s.ID Equals m.SchoolID
                     Order By s.Name
                     Select New With {
                     .School = s.Name,
                     .Matriculants = m.NumberMatriculants
                 }).ToList()
            Return Json(q, JsonRequestBehavior.AllowGet)
        End Using
    End Function

End Class
