Imports System.IO
Imports System.Globalization
Imports Microsoft.VisualBasic.FileIO
Imports System.Data
Imports ExcelDataReader

Public Module StatCsvImporter

    ' ---- Header aliases (normalized, flexible) ----
    Private ReadOnly H_SCHOOL As String() = {"medical school", "school", "institution"}
    Private ReadOnly H_STATE As String() = {"state"}
    Private ReadOnly H_NUM_APPS As String() = {"number of applications", "applications", "applications1"}
    Private ReadOnly H_IN_APP As String() = {"in-state application %", "in-state applications %", "in state application %", "in-state app %"}
    Private ReadOnly H_OUT_APP As String() = {"out-of-state application %", "out-of-state applications %", "out of state application %", "out-of-state app %"}
    Private ReadOnly H_NUM_MAT As String() = {"number of matriculants", "matriculants"}
    Private ReadOnly H_IN_MAT As String() = {"in-state matriculant %", "in state matriculant %", "in-state matriculants %"}
    Private ReadOnly H_OUT_MAT As String() = {"out-of-state matriculant %", "out of state matriculant %", "out-of-state matriculants %"}

    ' Main entry: returns a short summary for logs/UI
    Public Function ImportCalifornia(dataPath As String, ctx As DataContext) As String
        If String.IsNullOrWhiteSpace(dataPath) OrElse Not File.Exists(dataPath) Then
            Throw New FileNotFoundException("Data file Not found.", dataPath)
        End If

        Dim rows = ReadRows(dataPath).ToList()
        If rows.Count = 0 Then Return "No rows found."

        ' --- find the real header row (the one that contains both "State" and "Medical School") ---
        Dim headerRowIndex As Integer = -1
        For i = 0 To rows.Count - 1
            Dim norm = rows(i).Select(Function(s) Normalize(s)).ToArray()
            Dim hasState = norm.Any(Function(c) c = "state")
            Dim hasMedSchool = norm.Any(Function(c) c = "medical school")
            If hasState AndAlso hasMedSchool Then
                headerRowIndex = i
                Exit For
            End If
        Next
        If headerRowIndex = -1 Then
            Throw New InvalidDataException("Could Not locate header row containing 'State' and 'Medical School'.")
    End If

        Dim header = rows(headerRowIndex)
        Dim body = rows.Skip(headerRowIndex + 1)

        Dim index = BuildHeaderIndex(header)

        ' Positional mapping for this csv-not necessarily the best, but the multirow headers are making it hard to parse out each column
        Dim idxState As Integer = 0
        Dim idxSchool As Integer = 1
        Dim idxAppsNum As Integer = 2
        Dim idxInApp As Integer = 3
        Dim idxOutApp As Integer = 4
        ' 5,6 are gender % for applications (ignored)
        Dim idxMatNum As Integer = 7
        Dim idxInMat As Integer = 8
        Dim idxOutMat As Integer = 9
        ' 10,11 are gender % for matriculants (ignored)

        ' just in case we missed some refactored variable names
        Dim idxNumApps As Integer = idxAppsNum
        Dim idxNumMat As Integer = idxMatNum

        Dim tx = ctx.Database.BeginTransaction()

        Dim insertedSchools As Integer = 0
        Dim updatedApps As Integer = 0
        Dim insertedApps As Integer = 0
        Dim updatedMats As Integer = 0
        Dim insertedMats As Integer = 0
        Dim caRows As Integer = 0

        Try
            Dim lastState As String = ""

            For Each r In body
                If r Is Nothing OrElse r.Length = 0 Then Continue For

                ' ---- fill-down state ----
                Dim rawState = SafeGet(r, idxState)
                Dim state As String
                If HasText(rawState) Then
                    state = rawState.Trim()
                    lastState = state
                Else
                    state = lastState
                End If

                If Not state.Equals("CA", StringComparison.OrdinalIgnoreCase) Then Continue For

                ' ---- school ----
                Dim name = SafeGet(r, idxSchool).Trim()
                If Not HasText(name) Then Continue For
                If IsTotalRow(name) Then Continue For

                caRows += 1

                ' ---- numbers (default to 0) ----
                Dim numApps = ParseIntDefault0(SafeGet(r, idxAppsNum))
                Dim inApp = ParseDecDefault0(SafeGet(r, idxInApp))
                Dim outApp = ParseDecDefault0(SafeGet(r, idxOutApp))
                Dim numMat = ParseIntDefault0(SafeGet(r, idxMatNum))
                Dim inMat = ParseDecDefault0(SafeGet(r, idxInMat))
                Dim outMat = ParseDecDefault0(SafeGet(r, idxOutMat))

                ' ---- upsert school ----
                Dim school = ctx.Schools.FirstOrDefault(Function(s) s.Name = name)
                If school Is Nothing Then
                    school = New CaMedSchool With {.Name = name}
                    ctx.Schools.Add(school)
                    ctx.SaveChanges()
                    insertedSchools += 1
                End If

                ' ---- upsert applications (1 row per school) ----
                Dim app = ctx.Applications.FirstOrDefault(Function(a) a.SchoolID = school.ID)
                If app Is Nothing Then
                    app = New ApplicationStat With {
                        .SchoolID = school.ID,
                        .NumberApplications = numApps,
                        .InStateAppPercent = inApp,
                        .OutOfStateAppPercent = outApp
                    }
                    ctx.Applications.Add(app)
                    insertedApps += 1
                Else
                    app.NumberApplications = numApps
                    app.InStateAppPercent = inApp
                    app.OutOfStateAppPercent = outApp
                    updatedApps += 1
                End If

                ' ---- upsert matriculants (1 row per school) ----
                Dim mat = ctx.Matriculants.FirstOrDefault(Function(m) m.SchoolID = school.ID)
                If mat Is Nothing Then
                    mat = New MatriculantStat With {
                        .SchoolID = school.ID,
                        .NumberMatriculants = numMat,
                        .InStateMatPercent = inMat,
                        .OutOfStateMatPercent = outMat
                    }
                    ctx.Matriculants.Add(mat)
                    insertedMats += 1
                Else
                    mat.NumberMatriculants = numMat
                    mat.InStateMatPercent = inMat
                    mat.OutOfStateMatPercent = outMat
                    updatedMats += 1
                End If
            Next

            ctx.SaveChanges()
            tx.Commit()

            Return $"CA rows processed: {caRows}. Schools inserted: {insertedSchools}. Applications inserted/updated: {insertedApps}/{updatedApps}. Matriculants inserted/updated: {insertedMats}/{updatedMats}."
        Catch
            tx.Rollback()
            Throw
        End Try
    End Function

    ' ---------- helpers ----------
    Private Function FindExact(index As Dictionary(Of String, Integer), exact As String, err As String) As Integer
        Dim key = Normalize(exact)
        If index.ContainsKey(key) Then Return index(key)
        Throw New InvalidDataException(err)
    End Function

    Private Function FindContains(index As Dictionary(Of String, Integer), needles As IEnumerable(Of String), err As String) As Integer
        For Each n In needles
            Dim k = Normalize(n)
            Dim hit = index.Keys.FirstOrDefault(Function(x) x.Contains(k))
            If hit IsNot Nothing Then Return index(hit)
        Next
        Throw New InvalidDataException(err)
    End Function

    Private Function HasText(s As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(s)
    End Function

    Private Function IsTotalRow(name As String) As Boolean
        Dim n = name.Trim()
        If n.Equals("Total", StringComparison.OrdinalIgnoreCase) Then Return True
        If n.StartsWith("Total ", StringComparison.OrdinalIgnoreCase) Then Return True
        Return False
    End Function

    Private Function ParseIntDefault0(s As String) As Integer
        If String.IsNullOrWhiteSpace(s) Then Return 0
        s = s.Replace(",", "").Trim()
        Dim v As Integer
        If Integer.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, v) Then Return v
        Return 0
    End Function

    Private Function ParseDecDefault0(s As String) As Decimal
        If String.IsNullOrWhiteSpace(s) Then Return 0D
        s = s.Replace("%", "").Trim()
        Dim v As Decimal
        If Decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, v) Then Return v
        Return 0D
    End Function

    Private Function SafeGet(row As String(), idx As Integer) As String
        If idx < 0 OrElse idx >= row.Length Then Return ""
        Return If(row(idx), "").Trim()
    End Function

    Private Function BuildHeaderIndex(header As String()) As Dictionary(Of String, Integer)
        Dim dict = New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        For i = 0 To header.Length - 1
            Dim key = Normalize(header(i))
            If Not dict.ContainsKey(key) Then dict.Add(key, i)
        Next
        Return dict
    End Function

    Private Function RequireIndex(index As Dictionary(Of String, Integer), aliases As String(), err As String) As Integer
        For Each a In aliases
            Dim k = Normalize(a)
            If index.ContainsKey(k) Then Return index(k)
            ' tolerate extra words or year suffixes: pick a header that starts with our alias
            Dim hit = index.Keys.FirstOrDefault(Function(x) x.StartsWith(k, StringComparison.OrdinalIgnoreCase))
            If hit IsNot Nothing Then Return index(hit)
        Next
        Throw New InvalidDataException(err)
    End Function

    Private Function Normalize(s As String) As String
        If s Is Nothing Then Return ""
        s = s.ToLowerInvariant().Trim()
        Dim chars = s.Where(Function(c) Char.IsLetterOrDigit(c) OrElse Char.IsWhiteSpace(c) OrElse c = "%"c).ToArray()
        Dim cleaned = New String(chars)
        While cleaned.Contains("  ")
            cleaned = cleaned.Replace("  ", " ")
        End While
        Return cleaned
    End Function

    ' ---------- Readers (CSV/XLSX) ----------

    Private Function ReadRows(path As String) As IEnumerable(Of String())
        If LooksLikeXlsx(path) Then
            Return ReadXlsxRows(path)
        Else
            Return ReadCsvRows(path)
        End If
    End Function

    Private Function LooksLikeXlsx(path As String) As Boolean
        If path.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) Then Return True
        ' detect ZIP header "PK" for xlsx
        Using fs = File.OpenRead(path)
            Dim sig(1) As Byte
            If fs.Read(sig, 0, 2) = 2 Then
                If sig(0) = &H50 AndAlso sig(1) = &H4B Then Return True
            End If
        End Using
        Return False
    End Function

    Private Function ReadCsvRows(csvPath As String) As IEnumerable(Of String())
        Dim rows As New List(Of String())
        Using parser As New TextFieldParser(csvPath)
            parser.TextFieldType = FieldType.Delimited
            parser.SetDelimiters(",")
            parser.HasFieldsEnclosedInQuotes = True
            While Not parser.EndOfData
                rows.Add(parser.ReadFields())
            End While
        End Using
        Return rows
    End Function

    Private Function ReadXlsxRows(xlsxPath As String) As IEnumerable(Of String())
        Dim rows As New List(Of String())
        Using stream = File.OpenRead(xlsxPath)
            Using reader = ExcelReaderFactory.CreateReader(stream)
                Dim ds = reader.AsDataSet(New ExcelDataSetConfiguration With {
                .UseColumnDataType = False,
                .ConfigureDataTable = Function(cfg) New ExcelDataTableConfiguration With {.UseHeaderRow = False}
            })
                Dim table = ds.Tables(0) ' first sheet
                For Each dr As DataRow In table.Rows
                    Dim cols = New List(Of String)
                    For i = 0 To table.Columns.Count - 1
                        cols.Add(If(dr(i) IsNot Nothing, dr(i).ToString(), ""))
                    Next
                    rows.Add(cols.ToArray())
                Next
            End Using
        End Using
        Return rows
    End Function

End Module
