namespace HiitTrainer.Calculations;

public static class Helper
{
    public static TR Map<T, TR>(this T it, Func<T, TR> fun) => fun(it);

    public static T Do<T>(this T it, Action<T> act)
    {
        act(it);
        return it;
    }
}