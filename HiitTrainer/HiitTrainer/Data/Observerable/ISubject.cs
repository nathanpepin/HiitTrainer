namespace HiitTrainer.Data.Observerable;

public interface ISubject
{
    void Attach(IObserver observer);
    void Detach(IObserver observer);
    Task Notify();
}