using System.Text;
using FifteenPuzzle.PuzzleBoard;

namespace FifteenPuzzle.Dao;

public class BoardWriter : IFileWriter<List<Board>>
{
    private uint _counter;
    
    public void Write(in List<Board> boards)
    {
        string testsDirectoryPath = Path.Combine(Global.TestingDataDirPath);
        
        if (!Directory.Exists(testsDirectoryPath))
        {
            Directory.CreateDirectory(testsDirectoryPath);
        }
        
        foreach (var board in boards)
        {
            short columnSize = board.ColumnSize;
            short rowSize = board.RowSize;
            var sb = new StringBuilder();

            sb.AppendLine(rowSize + " " + columnSize);
            for (var i = 0; i < rowSize; i++)
            {
                for (var j = 0; j < columnSize; j++)
                {
                    int value = board.At(i, j);
                    string content = (j == columnSize - 1) ? value.ToString() : value + " ";
                    sb.Append(content);
                }

                if (i != rowSize - 1) sb.AppendLine();
            }

            string filename = CreateFilenameFor(board);
            File.WriteAllText(Path.Combine(testsDirectoryPath, filename), sb.ToString());
        }
    }

    private string CreateFilenameFor(Board board)
    {
        _counter++;
        var sb = new StringBuilder();
        sb.Append(board.RowSize);
        sb.Append('x');
        sb.Append(board.ColumnSize);
        
        sb.Append('_');
        
        int pathLength = board.PathLength;
        string number = pathLength.ToString("00");
        sb.Append(number);
        
        sb.Append('_');

        string idNumber = _counter.ToString("0000");
        sb.Append(idNumber);
        
        sb.Append(".txt");
        
        return sb.ToString();
    }
}