using BossPuzzle.Dao;
using BossPuzzle.PuzzleBoard;

namespace BossPuzzle;
using Dir = Board.Direction;

class Program
{
    public static void Main()
    {
        var sth = new FileFifteenPuzzleDao("test.file");
        Board board = sth.Read();
        board.Print();
        
        var bfsUDLR = new BFS(new[]
        {
            Dir.Up,
            Dir.Down,
            Dir.Left,
            Dir.Right
        });
        
        var solvedBoard = board.Solve(bfsUDLR);
        solvedBoard.Print();

        foreach (var item in solvedBoard.GetPath())
        {
            Console.Write(item.ToString()[0..1]);
        }
        Console.WriteLine();

        
        Console.ReadKey();
    }
}