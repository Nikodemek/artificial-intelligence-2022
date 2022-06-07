using MLP.Data.Interfaces;

namespace MLP.Data.Managers;

public class PlainDataFileManager : IFileManager<string>
{
    private readonly string _filePath;

    public PlainDataFileManager(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename)) throw new ArgumentException("Filename cannot be empty!", nameof(filename));

        _filePath = Path.Combine(Global.BaseDataDirPath, filename);
    }

    public string Read() => File.ReadAllText(_filePath);

    public void Write(string obj) => File.WriteAllText(_filePath, obj);
}