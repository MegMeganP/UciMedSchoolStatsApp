Imports System.ComponentModel.DataAnnotations

Public Class FeatureRequest
    <Key>
    Public Property Id As Integer

    <Required, StringLength(2000)>
    Public Property Description As String

    <Required, StringLength(200)>
    Public Property RequesterName As String

    <Required>
    Public Property RequestedAt As DateTime = DateTime.UtcNow

    <Required>
    Public Property IsCompleted As Boolean = False
End Class

