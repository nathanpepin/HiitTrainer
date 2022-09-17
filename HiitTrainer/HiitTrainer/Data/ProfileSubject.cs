using HiitTrainer.Data.Observerable;

namespace HiitTrainer.Data;

public class ProfileSubject : ISubject
{
    private readonly List<IObserver> _observers = new();

    public void Attach(IObserver observer)
    {
        if (_observers.Contains(observer)) return;

        _observers.Add(observer);
    }

    public void Detach(IObserver observer)
    {
        _observers.Remove(observer);
    }

    public async Task Notify()
    {
        await Task.WhenAll(_observers.Select(x => x.Update(this)));
    }
}