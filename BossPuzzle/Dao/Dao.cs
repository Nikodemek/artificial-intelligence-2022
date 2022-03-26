namespace BossPuzzle.Dao;

public interface IDao<T> : IFileWriter<T>, IFileReader<T>
{ }
