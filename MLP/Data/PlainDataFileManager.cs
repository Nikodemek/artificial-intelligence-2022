using MLP.Data.Interfaces;

namespace MLP.Data;

public class PlainDataFileManager : IFileManager<string>
{
    private readonly string _filePath;

    public PlainDataFileManager(string fileName)
    {
        if (String.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("FileName cannot be empty!", nameof(fileName));

        _filePath = Path.Combine(Global.BaseDataDirPath, fileName);
    }

    public string Read() => File.ReadAllText(_filePath);

    public void Write(string obj) => File.WriteAllText(_filePath, obj);
}