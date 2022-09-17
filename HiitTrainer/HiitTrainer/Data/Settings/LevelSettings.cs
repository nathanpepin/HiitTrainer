namespace HiitTrainer.Data;

public record LevelSettings
{
    public bool EditLevel { get; set; }
    public int Level { get; set; } = 1;
    public ProgressionType ProgressionType { get; set; } = ProgressionType.Seconds;
    public int ProgressionIncrease { get; set; } = 5;
    public int StartingExertion { get; set; } = 10;
    public int StartingRelease { get; set; } = 5;
    public int AdditionalExertion { get; set; }
    public int AdditionalRelease { get; set; }
    public bool IncreaseReleaseOnFailure { get; set; }
}