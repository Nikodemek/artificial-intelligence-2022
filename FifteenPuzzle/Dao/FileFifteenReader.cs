using FifteenPuzzle.PuzzleBoard;
using FifteenPuzzle.Utils;

namespace FifteenPuzzle.Dao;

public class FileFifteenReader : IFileReader<Board>
{
    private readonly string _fileName;
    private readonly string _filePath;

    public FileFifteenReader(string fileName)
    {
        if (String.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("FileName cannot be empty!", nameof(fileName));

        _fileName = fileName;
        _filePath = Path.Combine(Global.BaseDataDirPath, "tests", _fileName);

        if (!File.Exists(_filePath)) throw new FileNotFoundException("File not found!", _filePath);
    }

    public Board Read()
    {
        var data = File.ReadAllLines(_filePath);

        var list = data[0].Split(' ');
        int columnSize = Parser.ToInt32(list[0]);
        int rowSize = Parser.ToInt32(list[1]);
        var board = new short[columnSize, rowSize];

        for (var i = 0; i < columnSize; i++)
        {
            var row = data[i + 1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (var j = 0; j < rowSize; j++)
            {
                board[i, j] = Parser.ToInt16(row[j]);
            }
        }

        return new Board(board);
    }
}