using System.Text;
using BossPuzzle.PuzzleBoard;

namespace BossPuzzle.Dao;

public class InfoConverter
{
    private static int _def = -1;

    private Board _board;
    private IPuzzleSolver _solver;
    private string path;
    private int pathLength;

    public InfoConverter(Board board, IPuzzleSolver solver)
    {
        _board = board;
        _solver = solver;
        
        path = _board.GetPathFormatted();
        pathLength = path.Length;
    }
    
    public string PrepareBasicInfo()
    {
        if (!_board.IsValid()) return _def.ToString();
        
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(pathLength);
        stringBuilder.AppendLine();
        stringBuilder.Append(path);

        return stringBuilder.ToString();
    }
    
    public string PrepareAdditionalInfo()
    {
        var stringBuilder = new StringBuilder();
        
        pathLength = _board.IsValid() ? path.Length : _def;

        stringBuilder.AppendLine($"{pathLength}");
        stringBuilder.AppendLine();
        stringBuilder.AppendLine();
        stringBuilder.AppendLine(_solver.GetMaxDepthAchieved().ToString());
        stringBuilder.AppendLine($"{_solver.GetTimeConsumed():n3}");

        return stringBuilder.ToString();
    }
}