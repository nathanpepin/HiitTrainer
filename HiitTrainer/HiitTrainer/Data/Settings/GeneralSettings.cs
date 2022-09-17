using HiitTrainer.Calculations;

namespace HiitTrainer.Data;

public record GeneralSettings
{
    public int WorkoutLength { get; set; } = 60;

    public int Sets
    {
        get
        {
            var leftoverTime = WorkoutLength - Warmup - Cooldown;
            var setLength = (Reps * (Exertion + Release)) + Rest;
            return decimal
                .Divide(leftoverTime, setLength)
                .Map(Math.Round)
                .Map(Convert.ToInt32);
        }
    }

    public int Reps { get; set; } = 3;

    private int RepWorkoutTime => Warmup + (Sets * ((Reps * (Exertion + Release)) + Rest)) + Cooldown;

    public int RepsLastCount
    {
        get
        {
            var realWorkoutTime = RepWorkoutTime;

            var reps = Reps;

            while (realWorkoutTime > WorkoutLength)
            {
                realWorkoutTime -= Exertion + Release;
                reps--;
            }

            return reps;
        }
    }

    public int Release { get; set; } = 3;
    public int Exertion { get; set; } = 2;
    public int Cooldown { get; set; } = 5;

    public int CoolDownExtra => WorkoutLength - (RepWorkoutTime -
                                                 (Reps * (Exertion + Release) + Rest) +
                                                 RepsLastCount * (Exertion + Release) + Rest);

    public int CoolDownTotal
    {
        get => Cooldown + CoolDownExtra;
        set { }
    }

    public int Warmup { get; set; } = 5;
    public int Rest { get; set; } = 4;
    public bool SetReleaseCooldownManually { get; set; }
}

public record WorkoutDetails
{
    public Reps Reps { get; set; }
    public Sets Sets { get; set; }
    public Warmup Warmup { get; set; }
    public Exertion Exertion { get; set; }
    public Release Release { get; set; }
    public Cooldown Cooldown { get; set; }
}

public record Reps(int Value);

public record Sets(int Value);

public record Level(int Value);

public record Exertion(int Value);

public record Release(int Value);

public record Warmup(int Value);

public record Cooldown(int Value);

public record LevelProgression
{
    public int ExertionStart { get; set; }
    public int ReleaseStart { get; set; }
    public IProgression Progression { get; set; } = default!;
}

public interface IProgression
{
    int GetProgression(int level, decimal increase);
}

public enum ProgressionType
{
    Percentage,
    Seconds
}

public enum WorkoutMode
{
    Leveled,
    Manual
}