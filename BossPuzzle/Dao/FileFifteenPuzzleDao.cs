using BossPuzzle.PuzzleBoard;

namespace BossPuzzle.Dao;

public class FileFifteenPuzzleDao : IDao<Board>
{
    private static readonly string BaseDataDirPath = 
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "sise_2022");

    private readonly string _filename;
    
    public FileFifteenPuzzleDao(string filename)
    {
        this._filename = filename;
    }

    public Board Read()
    {
        var enumerableData = File.ReadLines(Path.Combine(BaseDataDirPath, _filename));
        var data = enumerableData.ToArray<string>();
        
        var list = data[0].Split(' ');
        var a = Convert.ToInt32(list[0]);
        var b = Convert.ToInt32(list[1]);
        var table = new int[a][];

        for (var i = 0; i < b; i++)
        {
            table[i] = new int[b];
        }
        
        for (var i = 0; i < data.Length; i++)
        {
            if (i == 0) continue;

            var row = data[i].Split(' ');
            for (var j = 0; j < a; j++)
            {
                var value = Convert.ToInt32(row[j]);
                table[i - 1][j] = value;
            }
        
        }

        return new Board(table);
    }
}