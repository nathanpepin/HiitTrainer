namespace HiitTrainer.Data;

#nullable disable
public record AppSettings
{
    public ProfileItem CurrentProfile { get; set; } = new();
}