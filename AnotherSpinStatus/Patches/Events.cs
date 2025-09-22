using AnotherSpinStatus.Classes;
using AnotherSpinStatus.Services;

namespace AnotherSpinStatus.Patches;

internal static class Events
{
    public static void Initialize()
    {
        Track.OnLoadedIntoTrack += TrackOnLoadedIntoTrack;
        Track.OnStartedPlayingTrack += TrackOnStartedPlayingTrack;
    }

    public static void Dispose()
    {
        Track.OnLoadedIntoTrack -= TrackOnLoadedIntoTrack;
        Track.OnStartedPlayingTrack -= TrackOnStartedPlayingTrack;
    }

    private static void TrackOnStartedPlayingTrack(PlayableTrackDataHandle dataHandle, PlayState[] states)
    {
        Patches.Overbeats = 0;
        SocketApi.Broadcast("Scene", "Playing");
    }

    private static void TrackOnLoadedIntoTrack(PlayableTrackDataHandle dataHandle, PlayState[] states)
    {
        MapInfo info = new(dataHandle.Data);
        SocketApi.Broadcast("MapData", info);
    }
}