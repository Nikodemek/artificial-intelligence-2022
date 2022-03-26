namespace BossPuzzle.Dao;

public class FileWriter : IFileWriter<string>
{
    private readonly string _fileName;
    private readonly string _filePath;

    public FileWriter(string fileName)
    {
        if (String.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("FileName cannot be empty!", nameof(fileName));

        _fileName = fileName;
        _filePath = Path.Combine(Global.BaseDataDirPath, _fileName);
    }

    public void Write(in string content)
    {
        File.WriteAllText(_filePath, content);
    }
}