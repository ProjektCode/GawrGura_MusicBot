Imports Discord
Imports Discord.Commands
Imports Discord.WebSocket

NotInheritable Class embedHandler


    Public Shared Async Function basicEmbed(title As String, description As String, color As Color) As Task(Of Embed)

        Dim embed = Await Task.Run(Function() ((New EmbedBuilder()) _
            .WithTitle(title) _
            .WithDescription(description) _
            .WithColor(color) _
            .WithCurrentTimestamp() _
            .Build()))

        Return embed
    End Function

    Public Shared Async Function queueEmbed(title As String, desc As String) As Task(Of Embed)
        Dim _util As New Utilities
        Dim embed = Await Task.Run(Function() ((New EmbedBuilder()) _
            .WithTitle(title) _
            .WithDescription(desc) _
            .WithColor(Color.Green) _
            .Build()))

        Return embed
    End Function

    Public Shared Async Function errorEmbed(_source As String, _error As String) As Task(Of Embed)

        Dim embed = Await Task.Run(Function() ((New EmbedBuilder()) _
            .WithTitle($"ERROR OCCURED FROM - {_source}") _
            .WithDescription($"**ERROR DETAILS**{Environment.NewLine}{_error}") _
            .WithColor(Color.Red) _
            .Build()))

        Return embed
    End Function



End Class
