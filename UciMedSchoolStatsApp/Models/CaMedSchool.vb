Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

<Table("CaMedSchool")>
Public Class CaMedSchool
    <Key>
    Public Property ID As Integer  ' PK
    <Required, MaxLength(255)>
    Public Property Name As String

    ' Tables that will have FK of the school ID
    Public Overridable Property Applications As ICollection(Of ApplicationStat)
    Public Overridable Property Matriculants As ICollection(Of MatriculantStat)

    Public Sub New()
        Applications = New List(Of ApplicationStat)()
        Matriculants = New List(Of MatriculantStat)()
    End Sub
End Class

