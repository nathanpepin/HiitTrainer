using System.Reflection;
using Blazored.LocalStorage;
using FluentResults;
using HiitTrainer.Data;

namespace HiitTrainer.Actions;

public class CookieIo : ICookieIo
{
    private readonly ILocalStorageService _localStorage;

    public CookieIo(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    private string KeyOrTypeName(string? key, MemberInfo t) => string.IsNullOrWhiteSpace(key) ? t.Name : key;
    private string ProfileKey(int profileId, string? key, MemberInfo t) => $"{profileId}_{KeyOrTypeName(key, t)}";

    public async Task<Result<T>> Read<T>(int profileId, string? key = "") where T : new()
    {
        var profileKey = ProfileKey(profileId, key, typeof(T));
        try
        {
            var result = await _localStorage.GetItemAsync<T>(profileKey);
            return (result is null) ? new T() : result;
        }
        catch
        {
            return Result.Fail($"Item not found: {profileKey}");
        }
    }

    public async Task<Result> Write<T>(int profileId, T value, string? key = "") where T : new()
    {
        if (value is null) return Result.Fail("Input cannot be null");

        try
        {
            await _localStorage.SetItemAsync(ProfileKey(profileId, key, typeof(T)), value);
        }
        catch
        {
            return Result.Fail("Failed to write result");
        }

        return Result.Ok();
    }


    public async Task<Result<IEnumerable<ProfileItem>>> ReadProfiles()
    {
        try
        {
            var result = await _localStorage.GetItemAsync<List<ProfileItem>>(nameof(ProfileItem));

            return result.Any() ? result : Result.Fail("No profiles found");
        }
        catch
        {
            return Result.Fail("There was an error retrieving profiles");
        }
    }

    public async Task<Result<ProfileItem>> ReadProfile(int profileId)
    {
        var profilesResult = await ReadProfiles();

        if (profilesResult.IsFailed) return profilesResult.ToResult();

        var profileSearchResult = profilesResult.Value.FirstOrDefault(x => x.Id == profileId);

        if (profileSearchResult is null) return Result.Fail($"Failed to find profile {profileId}");

        return profileSearchResult;
    }

    public async Task<Result> UpdateProfiles(IEnumerable<ProfileItem> profiles)
    {
        try
        {
            await _localStorage.SetItemAsync(nameof(ProfileItem), profiles.ToList());
            return Result.Ok();
        }
        catch
        {
            return Result.Fail("Failed to write profiles");
        }
    }

    public async Task<Result> UpdateProfileName(ProfileItem profileItem)
    {
        var profiles = await ReadProfiles();
        if (profiles.IsFailed) return Result.Fail("Failed to retrieve profiles");

        var profilesList = profiles.Value.ToList();

        if (profilesList.All(x => x.Id != profileItem.Id)) ;

        var profileIndex = profilesList.FindIndex(x => x.Id == profileItem.Id);
        profilesList[profileIndex] = profileItem;

        await UpdateProfiles(profilesList);

        return Result.Ok();
    }
}