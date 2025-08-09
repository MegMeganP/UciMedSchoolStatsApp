Imports System.IO
Imports System.Net
Imports System.Web.Mvc
Imports System.Web.Hosting

Public Class DataController
    Inherits Controller

    Private csvUrl As String = "https://www.aamc.org/media/79766/download?attachment"

    ' GET: /Data/
    Function Index(Optional force As Boolean = False) As ActionResult
        ' This will point to your project's App_Data folder in Solution Explorer
        Dim csvFilePath = HostingEnvironment.MapPath("~/App_Data/aamc_data.csv")

        ' Download CSV if missing or forced
        If force OrElse Not System.IO.File.Exists(csvFilePath) Then
            DownloadCsv(csvFilePath)
        End If

        ' Build a message for testing
        Dim message As String
        If System.IO.File.Exists(csvFilePath) Then
            Dim lineCount = System.IO.File.ReadAllLines(csvFilePath).Length
            message = $"CSV found at: {csvFilePath} ({lineCount} lines)"
        Else
            message = "CSV could not be downloaded."
        End If

        ' Return plain text for easy testing
        Return Content(message, "text/plain")
    End Function

    Private Sub DownloadCsv(filePath As String)
        ' Ensure App_Data exists
        Dim dir = Path.GetDirectoryName(filePath)
        If Not Directory.Exists(dir) Then
            Directory.CreateDirectory(dir)
        End If

        Using client As New WebClient()
            client.DownloadFile(csvUrl, filePath)
        End Using
    End Sub
End Class


