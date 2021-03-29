Imports Discord.Commands
Imports Discord
Imports Discord.WebSocket
Imports Victoria
Imports Microsoft.Extensions.DependencyInjection
Imports Victoria.Enums

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
    Public Async Function PlayAsync(<Remainder> ByVal searchQuery As String) As Task 'Figure out how to import this into audioManager
        Dim _lavaNode As LavaNode = serviceManager.provider.GetRequiredService(Of LavaNode)
        If String.IsNullOrWhiteSpace(searchQuery) Then
            Await ReplyAsync("Please provide search terms.")
            Return
        End If

        If Not _lavaNode.HasPlayer(Context.Guild) Then
            Await ReplyAsync("I'm not connected to a voice channel.")
            Return
        End If

        Dim queries = searchQuery.Split(";"c)
        For Each query In queries
            Dim searchResponse = If(Uri.IsWellFormedUriString(query, UriKind.Absolute), Await _lavaNode.SearchAsync(query), Await _lavaNode.SearchYouTubeAsync(query))
            If searchResponse.LoadStatus = LoadStatus.LoadFailed OrElse searchResponse.LoadStatus = LoadStatus.NoMatches Then
                Await ReplyAsync($"I wasn't able to find anything for `{query}`.")
                Return
            End If

            Dim player = _lavaNode.GetPlayer(Context.Guild)

            If player.PlayerState = PlayerState.Playing OrElse player.PlayerState = PlayerState.Paused Then
                If Not String.IsNullOrWhiteSpace(searchResponse.Playlist.Name) Then
                    For Each track In searchResponse.Tracks
                        player.Queue.Enqueue(track)
                    Next track

                    Await ReplyAsync($"Queued {searchResponse.Tracks.Count} tracks.")
                Else
                    Dim track = searchResponse.Tracks(0)
                    player.Queue.Enqueue(track)

                    Console.WriteLine($"Enqueued: {track.Title}")
                End If
            Else
                Dim track = searchResponse.Tracks(0)

                If Not String.IsNullOrWhiteSpace(searchResponse.Playlist.Name) Then
                    For i = 0 To searchResponse.Tracks.Count - 1
                        If i = 0 Then
                            Await player.PlayAsync(track)
                            Await ReplyAsync($"Now Playing: {track.Title}")
                        Else
                            player.Queue.Enqueue(searchResponse.Tracks(i))
                        End If
                    Next i

                    Await ReplyAsync($"Queued {searchResponse.Tracks.Count} tracks.")
                Else
                    Await player.PlayAsync(track)
                    Await ReplyAsync($"Now Playing: {track.Title}")
                End If
            End If
        Next query
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

    <Command("clear")>
    <Summary("Clears current queue")>
    Public Async Function cmdClear() As Task
        Dim msg = Context.Channel
        Dim g = Context.Guild
        Await msg.SendMessageAsync(Await audioManager.clearTracks(g))
    End Function

End Class
