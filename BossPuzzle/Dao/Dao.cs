using BossPuzzle.PuzzleBoard;

namespace BossPuzzle.Dao;

public interface IDao<T>
{
    T Read();
    void Write(T obj);
}