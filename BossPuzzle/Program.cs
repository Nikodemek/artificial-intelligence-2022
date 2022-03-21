using BossPuzzle.Dao;
using BossPuzzle.Utils;
using BossPuzzle.PuzzleBoard;

namespace BossPuzzle;
using Dir = Board.Direction;

class Program
{
    public static void Main()
    {
        /*var readFile = new FileFifteenPuzzleDao("test.file");
        var board = readFile.Read();*/
        var board = PuzzleGenerator.Generate(4, 4, 100);
        board.Print();
        Console.WriteLine($"Hammings distance = {board.Hammings}");

        IPuzzleSolver solver = new Hammings(50000);

        /*IPuzzleSolver solver = new BFS(new[]
        {
            Dir.Up,
            Dir.Down,
            Dir.Left,
            Dir.Right
        });*/

        /*IPuzzleSolver solver = new DFS(new[]
        {
            Dir.Up,
            Dir.Down,
            Dir.Left,
            Dir.Right
        });*/

        /*var rand = Random.Shared;
        var prevBoard = board;

        for (int i = 0; i < 10; i++)
        {
            Dir dir = (Dir)rand.Next(4);
            var currBoard = prevBoard.Move(dir);

            Console.WriteLine($"Hamming's distance = {currBoard.Hammings}");
            currBoard.Print();

            prevBoard = currBoard;
        }*/

        /*var hamm = new Hammings(1200);
        var solvedBoard = board.Solve(hamm);
        if (solvedBoard.IsValid()) Console.WriteLine("SOLVED!!");
        solvedBoard.Print();*/

        /*var saveFile = new FileFifteenPuzzleDao("test_sol.file");
        saveFile.Write(solvedBoard);*/

        Console.ReadKey();
    }
}