namespace BossPuzzle.Dao;

public interface IFileWriter<T>
{
    void Write(in T content);
}