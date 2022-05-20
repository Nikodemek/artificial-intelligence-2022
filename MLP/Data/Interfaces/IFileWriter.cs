namespace MLP.Data.Interfaces;

public interface IFileWriter<T>
{
    void Write(T obj);
}
