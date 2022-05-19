namespace MLP.Data.Interfaces;

public interface IFileManager<T> : IFileReader<T>, IFileWriter<T>
{ }