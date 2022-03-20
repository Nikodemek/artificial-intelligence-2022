using BossPuzzle.Dao;
using BossPuzzle.PuzzleBoard;

namespace BossPuzzle;
using Dir = Board.Direction;

class Program
{
    public static void Main()
    {
        /*var readFile = new FileFifteenPuzzleDao("test.file");
        Board board = readFile.Read();
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

        var saveFile = new FileFifteenPuzzleDao("test_sol.file");
        saveFile.Write(solvedBoard);*/
        
        var readFile = new FileFifteenPuzzleDao("test.file");
        Board board = readFile.Read();
        board.Print();
        
        var dfsUDLR = new DFS(new[]
        {
            Dir.Up,
            Dir.Down,
            Dir.Left,
            Dir.Right
        });
        
        var solvedBoard = board.Solve(dfsUDLR);
        solvedBoard.Print();
        
        var saveFile = new FileFifteenPuzzleDao("test_sol.file");
        saveFile.Write(solvedBoard);

        Console.ReadKey();
    }
}