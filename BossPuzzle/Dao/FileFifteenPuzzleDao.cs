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
        this._fileName = fileName;
        _filePath = Path.Combine(BaseDataDirPath, _fileName);
    }

    public Board Read()
    {
        var data = File.ReadAllLines(_filePath);
        
        var list = data[0].Split(' ');
        int columnSize = Parser.ToInt32(list[0]);
        int rowSize = Parser.ToInt32(list[1]);
        var table = new int[columnSize][];
        
        for (var i = 0; i < columnSize; i++)
        {
            var row = data[i + 1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            table[i] = new int[rowSize];

            for (var j = 0; j < rowSize; j++)
            {
                table[i][j] = Parser.ToInt32(row[j]);
            }
        
        }

        return new Board(table);
    }

    public void Write(Board board)
    {
        var stringBuilder = new StringBuilder();
        using var streamWriter = new StreamWriter(Path.Combine(BaseDataDirPath, _fileName));

        var count = board.GetPath().Length;

        stringBuilder.Append(count);
        stringBuilder.AppendLine();
        foreach (var item in board.GetPath())
        {
            stringBuilder.Append(item.ToString()[..1]);
        }
        streamWriter.Write(stringBuilder);
    }
}