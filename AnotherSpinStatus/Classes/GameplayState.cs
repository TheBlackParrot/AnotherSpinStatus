using System.Diagnostics.CodeAnalysis;

namespace AnotherSpinStatus.Classes;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public readonly struct GameplayState(PlayState playState)
{
    public int Score => playState.scoreState.TotalScore;
    public int BaseScore => playState.scoreState.CurrentTotals.baseScore;
    public int BaseScoreLost => playState.scoreState.CurrentTotals.baseScoreLost;
    public float Accuracy => Score / (float)((BaseScore + BaseScoreLost) * 4);
    public float Health => playState.health / (float)playState.MaxHealth;
    public int Combo => playState.combo;
    public int NotesHit => playState.scoreState.CurrentTotals.totalNotesHit;
    public int NotesMissed => playState.scoreState.cachedTotals.totalHittableNotes - playState.scoreState.cachedTotals.totalNotesHit;
    public string FullComboState => playState.fullComboState.ToString();
    public int NotesHitPerfectly => playState.scoreState.CurrentTotals.flawlessPlusCount;
}