Imports Discord.Commands
Imports Discord
Imports Discord.WebSocket
Imports Victoria

<Name("Music")>
Public Class cmdMusic
    Inherits ModuleBase(Of SocketCommandContext)

    <Command("join")>
    <Summary("Joins the voice channel you are currently in")>
    Public Async Function cmdJoin() As Task
        Dim msg = Context.Channel
        Dim g = Context.Guild
        Await msg.SendMessageAsync(Await audioManager.joinAsync(g, TryCast(Context.User, IVoiceState), TryCast(msg, ITextChannel)))
    End Function

    <Command("play")>
    <Summary("Plays song from YouTube")>
    Public Async Function cmdPlay(<Remainder> search As String) As Task
        Dim msg = Context.Channel
        Dim g = Context.Guild
        Await msg.SendMessageAsync(Await audioManager.playAsync(TryCast(Context.User, SocketGuildUser), g, search))
    End Function

    <Command("leave")>
    <Summary("Leaves voice channel")>
    Public Async Function cmdLeave() As Task
        Dim msg = Context.Channel
        Dim g = Context.Guild
        Await msg.SendMessageAsync(Await audioManager.leaveAsync(g))
    End Function

    <Command("volume")>
    <[Alias]("vol")>
    <Summary("Set the volume of the bot")>
    Public Async Function cmdVol(vol As Integer) As Task
        Dim msg = Context.Channel
        Dim g = Context.Guild
        Await msg.SendMessageAsync(Await audioManager.setVolumeAsync(g, vol))
    End Function

    <Command("pause")>
    <[Alias]("resume")>
    <Summary("Pauses/Resumes music player. This command is a toggle")>
    Public Async Function cmdPause() As Task
        Dim msg = Context.Channel
        Dim g = Context.Guild
        Await msg.SendMessageAsync(Await audioManager.togglePauseAsync(g))
    End Function

    <Command("skip")>
    <Summary("Skips the current song")>
    Public Async Function cmdSkip() As Task
        Dim msg = Context.Channel
        Dim g = Context.Guild
        Await msg.SendMessageAsync(Await audioManager.skipTrack(g))
    End Function

    <Command("list")>
    <[Alias]("queue")>
    <Summary("List all songs in the current queue")>
    Public Async Function cmdList() As Task
        Dim msg = Context.Channel
        Dim g = Context.Guild
        Await msg.SendMessageAsync(Await audioManager.listTracks(g))
    End Function

End Class
