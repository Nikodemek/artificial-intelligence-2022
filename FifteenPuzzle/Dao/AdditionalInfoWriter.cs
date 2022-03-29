using FifteenPuzzle.PuzzleBoard;
using System.Globalization;
using System.Text;

namespace FifteenPuzzle.Dao;

public class AdditionalInfoWriter : IFileWriter<RunInfo>
{
    private readonly string _fileName;
    private readonly string _filePath;

    public AdditionalInfoWriter(string fileName)
    {
        if (String.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("FileName cannot be empty!", nameof(fileName));

        _fileName = fileName;
        _filePath = Path.Combine(Global.BaseDataDirPath, _fileName);
    }

    public void Write(in RunInfo content)
    {
        int solutionLength = content.Solved ? content.PathLength : -1;
        int visited = content.VisitedStates;
        int processed = content.ProcessedStates;
        int maxDepth = content.MaxDepth;
        double executionTime = content.ExecutionTime;

        var sb = new StringBuilder();
        sb.Append(solutionLength).AppendLine();
        sb.Append(visited).AppendLine();
        sb.Append(processed).AppendLine();
        sb.Append(maxDepth).AppendLine();
        sb.Append(executionTime.ToString("0.000", CultureInfo.InvariantCulture));

        File.WriteAllText(_filePath, sb.ToString());
    }
}
