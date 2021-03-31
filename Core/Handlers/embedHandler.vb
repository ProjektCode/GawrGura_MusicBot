Imports Discord
Imports Discord.Commands
Imports Discord.WebSocket

NotInheritable Class embedHandler

    Public Shared Async Function basicEmbed(title As String, description As String, color As Color) As Task(Of Embed)

        Dim embed = Await Task.Run(Function() ((New EmbedBuilder()).WithTitle(title).WithDescription(description).WithColor(color).WithCurrentTimestamp().Build()))
        Return embed

    End Function

    Public Shared Async Function errorEmbed(_source As String, _error As String) As Task(Of Embed)
        Dim embed = New EmbedBuilder With {
        .Title = $"ERROR OCCURED FROM - {_source}",
        .Description = $"**ERROR DETAILS**{Environment.NewLine}{_error}",
        .Color = Color.Red
        }
    End Function



End Class
