Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

<Table("Matriculants")>
Public Class MatriculantStat
    <Key>
    Public Property Id As Integer

    <ForeignKey("School")>
    Public Property SchoolID As Integer

    Public Property NumberMatriculants As Integer?
    Public Property InStateMatPercent As Decimal?
    Public Property OutOfStateMatPercent As Decimal?

    Public Overridable Property School As CaMedSchool
End Class

