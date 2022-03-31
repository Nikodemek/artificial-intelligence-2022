using System.Text;
using FifteenPuzzle.PuzzleBoard;

namespace FifteenPuzzle.Dao;

public class BoardWriter: IFileWriter<List<Board>>
{
    private uint _counter;
    public void Write(in List<Board> boards)
    {
        foreach (var board in boards)
        {
            var columnSize = board.ColumnSize;
            var rowSize = board.RowSize;
            var sb = new StringBuilder();

            sb.AppendLine(rowSize + " " + columnSize);
            for (var i = 0; i < rowSize; i++)
            {
                for (var j = 0; j < columnSize; j++)
                {
                    var value = board.At(i, j);
                    var content = (j == columnSize - 1) ? value.ToString() : value + " ";
                    sb.Append(content);
                }

                if (i != rowSize - 1) sb.AppendLine();
            }

            var filename = CreateFilenameFor(board);

            var testsDirectoryPath = Path.Combine(Global.FinalDataDirPath);
            if (!Directory.Exists(testsDirectoryPath))
            {
                Directory.CreateDirectory(testsDirectoryPath);
            }
            File.WriteAllText(Path.Combine(testsDirectoryPath, filename), sb.ToString());
        }
    }

    private string CreateFilenameFor(Board board)
    {
        _counter++;
        StringBuilder sb = new StringBuilder();
        sb.Append(board.RowSize);
        sb.Append('x');
        sb.Append(board.ColumnSize);
        
        sb.Append('_');
        
        var pathLength = board.PathLength;
        var number = pathLength.ToString("00");
        sb.Append(number);
        
        sb.Append('_');
        
        var idNumber = _counter.ToString("0000");
        sb.Append(idNumber);
        
        sb.Append(".txt");
        
        return sb.ToString();
    }
}