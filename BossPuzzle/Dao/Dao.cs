namespace BossPuzzle.Dao;

public interface IDao<T>
{
    T Read();
}