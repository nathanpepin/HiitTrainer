namespace HiitTrainer.Data.Observerable;

public interface IObserver
{
    Task Update(ISubject subject);
}