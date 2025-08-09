Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

<Table("Applications")>
Public Class ApplicationStat
    <Key>
    Public Property Id As Integer

    <ForeignKey("School")>
    Public Property SchoolID As Integer

    Public Property NumberApplications As Integer?
    Public Property InStateAppPercent As Decimal?
    Public Property OutOfStateAppPercent As Decimal?

    Public Overridable Property School As CaMedSchool
End Class

