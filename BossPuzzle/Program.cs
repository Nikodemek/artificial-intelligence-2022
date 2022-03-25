using System.Text;
using BossPuzzle.Dao;
using BossPuzzle.Utils;
using BossPuzzle.PuzzleBoard;

namespace BossPuzzle;
using Dir = Board.Direction;
using Atype = Astar.Atype;

class Program
{
    public static void Main()
    {
        /*var readFile = new FileFifteenPuzzleDao("test.file");
        var board = readFile.Read();*/

        var board = PuzzleGenerator.Generate(4, 4, 18);
        board.Print();
        Console.WriteLine($"Hammings distance = {board.Hammings}");
        Console.WriteLine($"Manhattan distance = {board.Manhattans}");

        // IPuzzleSolver solver = new Astar(Atype.Manhattan);

        /*IPuzzleSolver solver = new BFS(new[]
        {
            Dir.Up,
            Dir.Down,
            Dir.Left,
            Dir.Right
        });*/

        IPuzzleSolver solver = new DFS(
            new[] {
            Dir.Up,
            Dir.Down,
            Dir.Left,
            Dir.Right 
            }, 
            19);

        var solvedBoard = board.Solve(solver);
        if (solvedBoard.IsValid()) Console.WriteLine("SOLVED!!");
        solvedBoard.Print();
        var stringBuilder = new StringBuilder();
        foreach (var direction in solvedBoard.GetPath())
        {
            stringBuilder.Append(direction.ToString()[0]);
        }
        Console.WriteLine($"Created {Board.instances} instances of Board.");

        Console.WriteLine(stringBuilder);

        /*var saveFile = new FileFifteenPuzzleDao("test_sol.file");
        saveFile.Write(solvedBoard);*/

        Console.ReadKey();
    }
}