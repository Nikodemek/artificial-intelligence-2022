using BossPuzzle.Dao;
using BossPuzzle.PuzzleBoard;

namespace BossPuzzle;

class Program
{
    public static void Main()
    {
        var sth = new FileFifteenPuzzleDao("test.file");
        Board board = sth.Read();
        board.Print();
        
        var solver = new BFS(new []
        {
            Board.Direction.Up,
            Board.Direction.Down,
            Board.Direction.Left,
            Board.Direction.Right
        });
        
        board = solver.Solve(board);
        board.Print();
        
        board.SolvePath.ForEach(x => Console.Write(x));
        Console.WriteLine();

        
        Console.ReadKey();
    }
}