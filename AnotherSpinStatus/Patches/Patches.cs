using AnotherSpinStatus.Classes;
using AnotherSpinStatus.Services;
using HarmonyLib;

namespace AnotherSpinStatus.Patches;

[HarmonyPatch]
internal class Patches
{
    [HarmonyPatch(typeof(Track), nameof(Track.StopTrack))]
    [HarmonyPostfix]
    private static void StopTrackPostfix()
    {
        SocketApi.Broadcast("Scene", "Menu");
    }

    [HarmonyPatch(typeof(Track), nameof(Track.HandlePauseGame))]
    [HarmonyPostfix]
    private static void HandlePauseGamePostfix()
    {
        SocketApi.Broadcast("Paused", true);
    }
    
    [HarmonyPatch(typeof(Track), nameof(Track.HandleUnpauseGame))]
    [HarmonyPostfix]
    private static void HandleUnpauseGamePostfix()
    {
        SocketApi.Broadcast("Paused", false);
    }

    [HarmonyPatch(typeof(PlayState), nameof(PlayState.Complete))]
    [HarmonyPostfix]
    private static void CompletePostfix(ref bool success)
    {
        SocketApi.Broadcast("Complete", success ? "Passed" : "Failed");
    }

    private static void BroadcastScoreUpdate(PlayState playState)
    {
        if (playState.previewState != PreviewState.NotPreview)
        {
            return;
        }

        SocketApi.Broadcast("Score", new GameplayState(playState));
    }
    
    [HarmonyPatch(typeof(SpinSectionLogic), nameof(SpinSectionLogic.UpdateSpinSectionState))]
    [HarmonyPatch(typeof(ScratchSectionLogic), nameof(ScratchSectionLogic.UpdateScratchSectionState))]
    [HarmonyPatch(typeof(FreestyleSectionLogic), nameof(FreestyleSectionLogic.UpdateFreestyleSectionState))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    private static bool ContinuousScoreEventPrefix(ref PlayState playState, out int __state)
    {
        __state = playState.scoreState.CurrentTotals.baseScore + playState.scoreState.CurrentTotals.baseScoreLost +
                  playState.health + playState.combo;
        return true;
    }
    
    [HarmonyPatch(typeof(SpinSectionLogic), nameof(SpinSectionLogic.UpdateSpinSectionState))]
    [HarmonyPatch(typeof(ScratchSectionLogic), nameof(ScratchSectionLogic.UpdateScratchSectionState))]
    [HarmonyPatch(typeof(FreestyleSectionLogic), nameof(FreestyleSectionLogic.UpdateFreestyleSectionState))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void ContinuousScoreEventPostfix(ref PlayState playState, int __state)
    {
        int newState = playState.scoreState.CurrentTotals.baseScore + playState.scoreState.CurrentTotals.baseScoreLost + 
                       playState.health + playState.combo;
        
        if (__state != newState)
        {
            BroadcastScoreUpdate(playState);
        }
    }

    [HarmonyPatch(typeof(Wheel), nameof(Wheel.OnNoteTapped))]
    [HarmonyPatch(typeof(Wheel), nameof(Wheel.OnSuccessfulBeat))]
    [HarmonyPostfix]
    private static void SimpleScoreEvent(ref PlayState playState)
    {
        BroadcastScoreUpdate(playState);
    }

    [HarmonyPatch(typeof(TrackGameplayLogic), nameof(TrackGameplayLogic.UpdateNoteState))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    private static bool UpdateNoteStatePrefix(ref PlayState playState, ref int noteIndex, out bool __state)
    {
        __state = playState.scoreState.GetNoteState(noteIndex).IsDoneWith;
        return true;
    }

    [HarmonyPatch(typeof(TrackGameplayLogic), nameof(TrackGameplayLogic.UpdateNoteState))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void UpdateNoteStatePostfix(ref PlayState playState, ref int noteIndex, bool __state)
    {
        NoteState noteState = playState.scoreState.GetNoteState(noteIndex);
        if (!(!__state && noteState.IsDoneWith))
        {
            return;
        }

        if (noteState.timingAccuracy == NoteTimingAccuracy.Failed)
        {
            BroadcastScoreUpdate(playState);
            return;
        }
        
        Note note = playState.noteData.GetNote(noteIndex);
        if (note.NoteType != NoteType.Match)
        {
            return;
        }
        
        BroadcastScoreUpdate(playState);
    }
}