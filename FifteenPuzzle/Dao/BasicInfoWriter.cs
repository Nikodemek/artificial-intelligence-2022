using FifteenPuzzle.PuzzleBoard;
using System.Text;

namespace FifteenPuzzle.Dao;

public class BasicInfoWriter : IFileWriter<RunInfo>
{
    private readonly string _fileName;
    private readonly string _filePath;

    public BasicInfoWriter(string fileName)
    {
        if (String.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("FileName cannot be empty!", nameof(fileName));

        _fileName = fileName;
        _filePath = Path.Combine(Global.FinalDataDirPath, _fileName);
    }

    public void Write(in RunInfo content)
    {
        int solutionLength = content.Solved ? content.PathLength : -1;
        string path = content.Path;

        var sb = new StringBuilder();
        sb.Append(solutionLength).AppendLine();
        sb.Append(path);

        var testsDirectoryPath = Path.Combine(Global.FinalDataDirPath);
        if (!Directory.Exists(testsDirectoryPath))
        {
            Directory.CreateDirectory(testsDirectoryPath);
        }
        File.WriteAllText(_filePath, sb.ToString());
    }
}