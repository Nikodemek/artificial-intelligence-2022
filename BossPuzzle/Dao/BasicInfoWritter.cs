using BossPuzzle.PuzzleBoard;
using System.Text;

namespace BossPuzzle.Dao;

public class BasicInfoWritter : IFileWriter<RunInfo>
{
    private readonly string _fileName;
    private readonly string _filePath;

    public BasicInfoWritter(string fileName)
    {
        if (String.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("FileName cannot be empty!", nameof(fileName));

        _fileName = fileName;
        _filePath = Path.Combine(Global.BaseDataDirPath, _fileName);
    }

    public void Write(in RunInfo content)
    {
        int solutionLength = content.Solved ? content.PathLength : -1;
        string path = content.Path;

        var sb = new StringBuilder();
        sb.Append(solutionLength).AppendLine();
        sb.Append(path);

        File.WriteAllText(_filePath, sb.ToString());
    }
}