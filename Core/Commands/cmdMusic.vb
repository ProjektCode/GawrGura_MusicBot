Imports Discord.Commands
Imports Discord
Imports Discord.WebSocket

<Name("Music")>
Public Class cmdMusic
    Inherits ModuleBase(Of SocketCommandContext)

    <Command("join")>
    <Summary("Joins the voice channel you are currently in")>
    Public Async Function cmdJoin() As Task
        Await Context.Channel.SendMessageAsync(Await audioManager.joinAsync(Context.Guild, TryCast(Context.User, IVoiceState), TryCast(Context.Channel, ITextChannel)))

    End Function

    <Command("play")>
    <Summary("Plays song from YouTube")>
    Public Async Function cmdPlay(<Remainder> search As String) As Task
        Await Context.Channel.SendMessageAsync(Await audioManager.playAsync(TryCast(Context.User, SocketGuildUser), Context.Guild, search))

    End Function

    <Command("leave")>
    <Summary("Leaves voice channel")>
    Public Async Function cmdLeave() As Task
        Await Context.Channel.SendMessageAsync(Await audioManager.leaveAsync(Context.Guild))
    End Function

    <Command("volume")>
    <[Alias]("vol")>
    <Summary("Set the volume of the bot")>
    Public Async Function cmdVol(vol As Integer) As Task
        Await Context.Channel.SendMessageAsync(Await audioManager.setVolumeAsync(Context.Guild, vol))
    End Function

    <Command("pause")>
    <[Alias]("resume")>
    <Summary("Pauses/Resumes music player. This command is a toggle")>
    Public Async Function cmdPause() As Task
        Await Context.Channel.SendMessageAsync(Await audioManager.togglePauseAsync(Context.Guild))

    End Function




End Class
