using FluentResults;
using HiitTrainer.Data;

namespace HiitTrainer.Actions;

public interface ICookieIo
{
    Task<Result<T>> Read<T>(int profileId, string? key = null) where T : new();
    Task<Result> Write<T>(int profileId, T value, string? key = null) where T : new();
    Task<Result<IEnumerable<ProfileItem>>> ReadProfiles();
    Task<Result<ProfileItem>> ReadProfile(int profileId);
    Task<Result> UpdateProfiles(IEnumerable<ProfileItem> profiles);
    Task<Result> UpdateProfileName(ProfileItem profileItem);
}