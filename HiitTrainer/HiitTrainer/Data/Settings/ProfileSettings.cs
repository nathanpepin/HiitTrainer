namespace HiitTrainer.Data;

public record ProfileSettings
{
    public int Id { get; set; } = 0;
    public string WarmupName { get; set; } = "Warmup";
    public string ExertionName { get; set; } = "Exertion";
    public string ReleaseName { get; set; } = "Release";
    public string RestName { get; set; } = "Rest";
    public string CoolDownName { get; set; } = "Cooldown";
    public string RepFailName { get; set; } = "Rep failed";
}