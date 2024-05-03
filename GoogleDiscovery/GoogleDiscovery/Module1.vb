Imports System
Imports System.Threading.Tasks
Imports Google
Imports Google.Apis.Discovery.v1
Imports Google.Apis.Discovery.v1.Data
Imports Google.Apis.Services
Module Module1
    Dim ApiKey As String = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
    Dim ResultName As String = IO.Path.Combine(My.Computer.FileSystem.CurrentDirectory, "GoogleApi.html")
    Dim Title As String = "Google API list"

    Sub Main()
        Console.WriteLine("Discovery API Sample")
        Console.WriteLine("====================")
        Dim ApiList As List(Of Tuple(Of String, String, String, String))
        Try
            ApiList = Run().Result
        Catch ex As AggregateException
            For Each e In ex.InnerExceptions
                Console.WriteLine("ERROR: " & e.Message)
            Next
        End Try
        My.Computer.FileSystem.WriteAllText(ResultName, $"<html><title>{Title}</title><style>{CSS}</style><body><table>", False)
        ApiList.Sort(Function(X, Y) X.Item2.CompareTo(Y.Item2))
        Dim Body As New Text.StringBuilder
        ApiList.ForEach(Sub(X) Body.Append($"<tr><td>{X.Item2}</td><td><a href='{X.Item3}'>{X.Item3}</a></td><td><a href='{X.Item4}'>{X.Item1}</a></td></tr>"))
        My.Computer.FileSystem.WriteAllText(ResultName, Body.ToString, True)
        My.Computer.FileSystem.WriteAllText(ResultName, "</table></body></html>", True)
        Console.WriteLine($"Result write to {ResultName}")
        Console.ReadKey()
    End Sub

    Async Function Run() As Task(Of List(Of Tuple(Of String, String, String, String)))
        Dim ApiList As New List(Of Tuple(Of String, String, String, String))
        Dim Service = New DiscoveryService(New BaseClientService.Initializer With {
            .ApplicationName = Title,
            .ApiKey = ApiKey
        })
        Console.WriteLine("Executing a list request...")
        Dim Result = Await Service.Apis.List().ExecuteAsync()
        If Result.Items IsNot Nothing Then
            Dim I As Integer = 0
            For Each Api As DirectoryList.ItemsData In Result.Items
                I = I + 1
                Console.WriteLine($"{I}. {Api.Id} - {Api.Title}")
                ApiList.Add(New Tuple(Of String, String, String, String)(Api.Id, Api.Title, Api.DocumentationLink, Api.DiscoveryRestUrl))
            Next
        End If
        Return ApiList
    End Function

    Dim CSS As String = "table {
  counter-reset: rowNumber;
}
table tr::before {
  display: table-cell;
  counter-increment: rowNumber;
  content: counter(rowNumber) '.';
  padding-right: 0.3em;
  text-align: right;
}"


End Module
