namespace MKW.Common
{
    public record class LeftRightPair<T>(
        T? Left,
        T? Right
    ) where T : class;
}
