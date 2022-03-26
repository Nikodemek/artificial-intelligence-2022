namespace BossPuzzle.Dao;

public interface IDao<T>
{
    T Read();
    void Write(in T obj);
}