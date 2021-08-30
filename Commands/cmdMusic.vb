Imports Discord
Imports Discord.WebSocket
Imports Discord.Commands
Imports Victoria
Imports Victoria.Enums
Imports Figgle
Imports Microsoft.Extensions.DependencyInjection

<Name("Music")>
Public Class cmdMusic
    Inherits ModuleBase(Of SocketCommandContext)
    Dim _lavaNode As LavaNode = serviceManager.provider.GetRequiredService(Of LavaNode)
    Dim utils As New Utilities

    <Command("join")>
    <Summary("Joins your voice channel.")>
    Public Async Function cmdJoin() As Task
        Dim msg = Context.Channel
        Dim g = Context.Guild
        Await msg.SendMessageAsync(Await audioManager.joinAsync(g, TryCast(Context.User, IVoiceState), TryCast(msg, ITextChannel)))
    End Function

    <Command("play")>
    <Summary("Plays song - Can be done by text search or URL.")>
    Public Async Function PlayAsync(<Remainder> ByVal searchQuery As String) As Task
        'Figure out how to import this into audioManager - It does nothing when added into audioManager but when in the command
        'class it works fine
        If String.IsNullOrWhiteSpace(searchQuery) Then
            Await ReplyAsync("Please provide search terms.")
            Return
        End If

        If Not _lavaNode.HasPlayer(Context.Guild) Then
            Await ReplyAsync("I'm not connected to a voice channel.")
            Return
        End If

        Dim searchResponse = If(Uri.IsWellFormedUriString(searchQuery, UriKind.Absolute), Await _lavaNode.SearchAsync(searchQuery), Await _lavaNode.SearchYouTubeAsync(searchQuery))

        If searchResponse.LoadStatus = searchResponse.LoadStatus.LoadFailed OrElse searchResponse.LoadStatus = searchResponse.LoadStatus.NoMatches Then
            Await ReplyAsync($"I wasn't able to find anything for `{searchQuery}`.")
            Return
        End If

        Dim player = _lavaNode.GetPlayer(Context.Guild)

        If player.PlayerState = PlayerState.Playing OrElse player.PlayerState = PlayerState.Paused Then
            If Not String.IsNullOrWhiteSpace(searchResponse.Playlist.Name) Then
                For Each track In searchResponse.Tracks
                    player.Queue.Enqueue(track)
                Next track
                Await ReplyAsync($"Queued {searchResponse.Tracks.Count} tracks.")
                Await loggingManager.LogInformationAsync("audio", $"Queued {searchResponse.Tracks.Count} tracks.")
            Else
                Dim track = searchResponse.Tracks(0)
                player.Queue.Enqueue(track)
                Await ReplyAsync($"Added {track.Title} to the queue")
                Await loggingManager.LogInformationAsync("audio", $"Enqueued: {track.Title}")
            End If
        Else
            Dim track = searchResponse.Tracks(0)

            If Not String.IsNullOrWhiteSpace(searchResponse.Playlist.Name) Then
                For i = 0 To searchResponse.Tracks.Count - 1
                    If i = 0 Then
                        Await player.PlayAsync(track)
                        Await ReplyAsync($"Now Playing: **{track.Title}**")
                    Else
                        player.Queue.Enqueue(searchResponse.Tracks(i))
                    End If
                Next i

                Await ReplyAsync($"Queued {searchResponse.Tracks.Count} tracks.")
            Else
                Await player.PlayAsync(track)
                Await ReplyAsync($"Now Playing: **{track.Title}**")
            End If
        End If

    End Function

    <Command("leave")>
    <Summary("Leaves voice channel.")>
    Public Async Function cmdLeave() As Task
        Dim msg = Context.Channel
        Dim g = Context.Guild
        Await msg.SendMessageAsync(Await audioManager.leaveAsync(g, TryCast(Context.User, IVoiceState)))
    End Function

    <Command("volume")>
    <[Alias]("vol")>
    <Summary("Set the volume of the bot. Default is 25.")>
    Public Async Function cmdVol(vol As Integer) As Task
        Dim msg = Context.Channel
        Dim g = Context.Guild
        Await msg.SendMessageAsync(Await audioManager.setVolumeAsync(g, vol))
    End Function

    <Command("pause")>
    <[Alias]("resume")>
    <Summary("A toggle command to pause/resume.")>
    Public Async Function cmdPause() As Task
        Dim msg = Context.Channel
        Dim g = Context.Guild
        Await msg.SendMessageAsync(Await audioManager.togglePauseAsync(g))
    End Function

    <Command("skip")>
    <Summary("Skips the current song.")>
    Public Async Function cmdSkip() As Task
        Dim msg = Context.Channel
        Dim g = Context.Guild
        Await msg.SendMessageAsync(Await audioManager.skipTrack(g))
    End Function

    <Command("queue")>
    <[Alias]("list")>
    <Summary("Lists all songs in the current queue.")>
    Public Async Function cmdList() As Task
        Dim msg = Context.Channel
        Dim g = Context.Guild
        Dim player = _lavaNode.GetPlayer(g)


        If player.Queue.Count < 1 And player.Track IsNot Nothing Then
            audioManager.noQueue = True
        Else
            audioManager.noQueue = False
        End If

        If audioManager.noQueue = True Then
            Dim embed1 As New EmbedBuilder With {
            .Title = "Current Song",
            .Description = Await audioManager.listTracks(g),
            .Color = New Color(utils.randomEmbedColor),
            .ThumbnailUrl = Context.Client.CurrentUser.GetAvatarUrl,
            .Footer = New EmbedFooterBuilder With {
                .Text = "Current Song Embed",
                .IconUrl = Context.Client.CurrentUser.GetAvatarUrl
            }
         }
            Await msg.SendMessageAsync("", False, embed1.Build)
        Else
            If audioManager.noQueue = False Then
                Dim embed2 As New EmbedBuilder With {
                    .Title = "Current Queue",
                    .Description = Await audioManager.listTracks(g),
                    .Color = New Color(utils.randomEmbedColor),
                    .ThumbnailUrl = Context.Client.CurrentUser.GetAvatarUrl,
                    .Footer = New EmbedFooterBuilder With {
                        .Text = "Queue Embed",
                        .IconUrl = Context.Client.CurrentUser.GetAvatarUrl
                    }
                }
                Await msg.SendMessageAsync("", False, embed2.Build)
            End If
        End If



    End Function

    <Command("clear")>
    <Summary("Clears current queue.")>
    Public Async Function cmdClear() As Task
        Dim msg = Context.Channel
        Dim g = Context.Guild
        Await msg.SendMessageAsync(Await audioManager.clearTracks(g))
        Threading.Thread.Sleep(1500)
        Console.Clear()
        utils.setBanner("/ Gawr Gura \", ConsoleColor.Cyan, ConsoleColor.Green)
    End Function

    <Command("stop")>
    <Summary("Stops playback completely.")>
    Public Async Function cmdStop() As Task
        Dim msg = Context.Channel
        Dim g = Context.Guild
        Await msg.SendMessageAsync(Await audioManager.stopAsync(g))
    End Function

    <Command("restart")>
    <Summary("Repeats the current song.")>
    Public Async Function cmdRestart() As Task
        Dim msg = Context.Channel
        Dim g = Context.Guild
        Await msg.SendMessageAsync(Await audioManager.restartAsync(g))
    End Function

    <Command("seek")>
    <Summary("Seek to a certain point in the current song.")>
    <[Alias]("sk")>
    Public Async Function cmdSeek(<Remainder> time As TimeSpan) As Task
        Dim msg = Context.Channel
        Dim g = Context.Guild
        Await msg.SendMessageAsync(Await audioManager.seekAsync(g, time))
    End Function

    <Command("shuffle")>
    <Summary("Shuffles current queue.")>
    Public Async Function cmdShuffle() As Task
        Dim chnl = Context.Channel
        Dim msg = Context.Message
        Dim g = Context.Guild
        Dim u As IVoiceState = Context.User
        Await chnl.SendMessageAsync(Await audioManager.shuffleAsync(g, msg, u))
    End Function

    <Command("np")>
    <Summary("Shows the current song.")>
    Public Async Function cmdNowPlaying() As Task
        Dim chnl = Context.Channel
        Dim g = Context.Guild

        Await chnl.SendMessageAsync(Await audioManager.nowPlayingAsync(g))
    End Function

    <Command("repeat")>
    <Summary("repeats current song")>
    Public Async Function cmdRepeat() As Task
        Dim chnl = Context.Channel
        Dim g = Context.Guild
        Await chnl.SendMessageAsync(Await audioManager.repeatAsync(g))
    End Function

End Class
