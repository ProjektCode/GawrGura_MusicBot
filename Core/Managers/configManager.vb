Imports System.IO
Imports Newtonsoft.Json

NotInheritable Class configManager
    Public ReadOnly Property token As String
    Public ReadOnly Property prefix As String

    <JsonIgnore> Private Shared Filename As String = "config.json"

    <JsonConstructor>
    Public Sub New(Token As String, Prefix As String)
        Me.token = Token
        Me.prefix = Prefix
    End Sub


    Public Shared Function Load() As configManager
        If File.Exists(Filename) Then
            Dim Json As String = File.ReadAllText(Filename)
            Return JsonConvert.DeserializeObject(Of configManager)(Json)

        End If
        Return Create()
    End Function

    Private Shared Function Create() As configManager
        Dim RawToken As String = Nothing
        Dim RawPrefix As String = Nothing

        Do While String.IsNullOrEmpty(RawToken)
            Console.Write("Enter Token: ")
            RawToken = Console.ReadLine
        Loop
        Do While String.IsNullOrEmpty(RawPrefix)
            Console.Write("Enter Prefix: ")
            RawPrefix = Console.ReadLine
        Loop



        Dim Config As New configManager(RawToken, RawPrefix)
        Config.Save()
        Return Config
    End Function

    Private Sub Save()
        Dim Json As String = JsonConvert.SerializeObject(Me, Formatting.Indented)
        File.WriteAllText(Filename, Json)
    End Sub

End Class

Public Structure botConfig
    Private privateToken As String
    <JsonProperty("token")>
    Public Property Token() As String
        Get
            Return privateToken
        End Get
        Private Set(value As String)
            privateToken = value
        End Set
    End Property
    Private privatePrefix As String
    <JsonProperty("prefix")>
    Public Property Prefix() As String
        Get
            Return privatePrefix
        End Get
        Private Set(value As String)
            privatePrefix = value
        End Set
    End Property
End Structure
