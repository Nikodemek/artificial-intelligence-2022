namespace BossPuzzle;

public interface IDao<T>
{
    T Read();
}