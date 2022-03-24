using System.Text;
using BossPuzzle.PuzzleBoard;
using BossPuzzle.Utils;

namespace BossPuzzle.Dao;

public class FileFifteenPuzzleDao : IDao<Board>
{
    private static readonly string BaseDataDirPath = 
        Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments
                ), 
            "sise_2022"
            );

    private readonly string _fileName;
    private readonly string _filePath;
    
    public FileFifteenPuzzleDao(string fileName)
    {
        _fileName = fileName;
        _filePath = Path.Combine(BaseDataDirPath, _fileName);
    }

    public Board Read()
    {
        var data = File.ReadAllLines(_filePath);
        
        var list = data[0].Split(' ');
        int columnSize = Parser.ToInt32(list[0]);
        int rowSize = Parser.ToInt32(list[1]);
        var board = new short[columnSize][];
        
        for (var i = 0; i < columnSize; i++)
        {
            var row = data[i + 1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            board[i] = new short[rowSize];

            for (var j = 0; j < rowSize; j++)
            {
                board[i][j] = Parser.ToInt16(row[j]);
            }
        
        }

        return new Board(board);
    }

    public void Write(in Board board)
    {
        var path = board.GetPath();
        int pathLength = path.Length;

        var stringBuilder = new StringBuilder();
        stringBuilder.Append(pathLength);
        stringBuilder.AppendLine();
        foreach (var item in path)
        {
            stringBuilder.Append(item.ToString()[0]);
        }

        File.WriteAllText(_filePath, stringBuilder.ToString());
    }
}