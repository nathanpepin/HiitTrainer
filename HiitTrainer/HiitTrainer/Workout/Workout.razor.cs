using System.Security.Cryptography;
using HiitTrainer.Actions;
using HiitTrainer.Data;
using Microsoft.AspNetCore.Components;

namespace HiitTrainer.Workout;

public partial class Workout
{
    [Inject] private AppSettings AppSettings { get; set; }
    [Inject] private ICookieIo CookieIo { get; set; } = default!;

    private bool _started, _saveProgressEnabled, _lastRep;
    CurrentTimer _currentTimer = CurrentTimer.Warmup;
    private int _total, _warmup, _exertion, _release, _rest, _cooldown, _reps = 1, _sets = 1, _failedReps = 0, _repTotal;

    private LevelSettings LevelSettings { get; set; } = new();
    private GeneralSettings GeneralSettings { get; set; } = new();
    private ProfileSettings ProfileSettings { get; set; } = new();
    private AdvancedSettings AdvancedSettings { get; set; } = new();
    [Parameter] public string? SelectedProfile { get; set; }

    private int _selectedProfile;

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        _selectedProfile = int.TryParse(SelectedProfile, out var result) ? result : 0;

        var profileResult = await CookieIo.ReadProfile(_selectedProfile);

        //TODO: Change this to go to an error page instead
        if (profileResult.IsSuccess)
            AppSettings.CurrentProfile = profileResult.Value;
        else
            throw new Exception("Profile not found");

        var resultLevelSettings = CookieIo.Read<LevelSettings>(_selectedProfile);
        var resultGeneralSettings = CookieIo.Read<GeneralSettings>(_selectedProfile);
        var resultProfileSettings = CookieIo.Read<ProfileSettings>(_selectedProfile);
        var resultAdvancedSettings = CookieIo.Read<AdvancedSettings>(_selectedProfile);

        await Task.WhenAll(resultLevelSettings, resultGeneralSettings, resultProfileSettings, resultAdvancedSettings);

        if (resultLevelSettings.Result.IsSuccess) LevelSettings = resultLevelSettings.Result.Value;
        if (resultGeneralSettings.Result.IsSuccess) GeneralSettings = resultGeneralSettings.Result.Value;
        if (resultProfileSettings.Result.IsSuccess) ProfileSettings = resultProfileSettings.Result.Value;
        if (resultAdvancedSettings.Result.IsSuccess) AdvancedSettings = resultAdvancedSettings.Result.Value;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

#pragma warning disable CS4014
        Ticker();
#pragma warning restore CS4014
    }

    private async Task Ticker()
    {
        while (true)
        {
            if (_total == GeneralSettings.WorkoutLength)
            {
                _started = false;
            }

            if (_started)
            {
                _total++;

                switch (_currentTimer)
                {
                    case CurrentTimer.Warmup:
                        _warmup++;

                        if (_warmup == GeneralSettings.Warmup)
                        {
                            _repTotal = (_sets == GeneralSettings.Sets)
                                ? GeneralSettings.RepsLastCount
                                : GeneralSettings.Reps;

                            _currentTimer = CurrentTimer.Exertion;
                        }

                        break;
                    case CurrentTimer.Exertion:
                        _exertion++;

                        if (_exertion == GeneralSettings.Exertion)
                            _currentTimer = CurrentTimer.Release;


                        break;
                    case CurrentTimer.Release:
                        _release++;

                        if (_release == GeneralSettings.Release)
                        {
                            _reps++;

                            if (_sets == GeneralSettings.Sets && _reps + 1 >= GeneralSettings.RepsLastCount)
                                _lastRep = true;

                            if (_sets == GeneralSettings.Sets && _reps >= GeneralSettings.RepsLastCount)
                            {
                                _reps--;
                                _currentTimer = CurrentTimer.Rest;
                            }
                            else if (_reps <= GeneralSettings.Reps)
                            {
                                _exertion = 0;
                                _release = 0;
                                _currentTimer = CurrentTimer.Exertion;
                            }
                            else
                            {
                                _reps = GeneralSettings.Reps;
                                _currentTimer = CurrentTimer.Rest;
                            }
                        }

                        break;
                    case CurrentTimer.Rest:
                        _rest++;

                        if (_rest == GeneralSettings.Rest)
                        {
                            _sets++;

                            if (_sets <= GeneralSettings.Sets)
                            {
                                _reps = 1;
                                _exertion = 0;
                                _release = 0;
                                _rest = 0;
                                _repTotal = (_sets == GeneralSettings.Sets)
                                    ? GeneralSettings.RepsLastCount
                                    : GeneralSettings.Reps;

                                _currentTimer = CurrentTimer.Exertion;
                            }
                            else
                            {
                                _sets = GeneralSettings.Sets;
                                _currentTimer = CurrentTimer.Cooldown;
                            }
                        }

                        break;
                    case CurrentTimer.Cooldown:
                        _cooldown++;

                        if (_cooldown == GeneralSettings.CoolDownTotal)
                            _currentTimer = CurrentTimer.End;

                        break;
                    case CurrentTimer.End:
                        _started = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            StateHasChanged();
            await Task.Delay(1000);
        }
    }

    private bool RepFailedDisabled => _currentTimer is > CurrentTimer.Warmup and < CurrentTimer.Cooldown;

    private void Reset()
    {
        _total = 0;
        _warmup = 0;
        _exertion = 0;
        _release = 0;
        _rest = 0;
        _cooldown = 0;
        _currentTimer = CurrentTimer.Warmup;
        _failedReps = 0;
        _reps = 1;
        _sets = 1;
        _lastRep = false;
    }

    private CurrentTimer GetNextState()
    {
        if (_currentTimer == CurrentTimer.End) return CurrentTimer.End;

        _currentTimer += 1;
        return _currentTimer;
    }

    enum CurrentTimer
    {
        Warmup,
        Exertion,
        Release,
        Rest,
        Cooldown,
        End
    }
}