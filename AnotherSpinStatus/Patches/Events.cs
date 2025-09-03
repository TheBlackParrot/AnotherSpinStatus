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

    private static void TrackOnStartedPlayingTrack(PlayableTrackDataHandle dataHandle, PlayState[] states)
    {
        SocketApi.Broadcast("Scene", "Playing");
    }

    private static void TrackOnLoadedIntoTrack(PlayableTrackDataHandle dataHandle, PlayState[] states)
    {
        MapInfo info = new(dataHandle.Data);
        SocketApi.Broadcast("MapData", info);
    }
}