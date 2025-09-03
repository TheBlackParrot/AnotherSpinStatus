namespace AnotherSpinStatus.Classes;

public readonly struct GameplayState(PlayState playState)
{
    public int Score => playState.scoreState.TotalScore;
    public int BaseScore => playState.scoreState.CurrentTotals.baseScore;
    public int BaseScoreLost => playState.scoreState.CurrentTotals.baseScoreLost;
    public float Accuracy => BaseScore / (float)(BaseScore + BaseScoreLost);
    public float Health => playState.health / (float)playState.MaxHealth;
    public int Combo => playState.combo;
    public int NotesHit => playState.scoreState.CurrentTotals.totalNotesHit;
    public int NotesMissed => playState.scoreState.TotalMissedNotes;
}