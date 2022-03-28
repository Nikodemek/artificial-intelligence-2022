using System.Text;
using BossPuzzle.PuzzleBoard;

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

    public static string PrepareAdditionalInfo(Board board, IPuzzleSolver solver)
    {
        var stringBuilder = new StringBuilder();

        int pathLength;
        string path = board.GetPathFormatted();
        pathLength = board.IsValid() ? path.Length : -1;

        stringBuilder.AppendLine($"{pathLength}");
        stringBuilder.AppendLine();
        stringBuilder.AppendLine();
        stringBuilder.AppendLine(solver.GetMaxDepthAchieved().ToString());
        stringBuilder.AppendLine($"{solver.GetTimeConsumed():n3}");

        return stringBuilder.ToString();
    }
}