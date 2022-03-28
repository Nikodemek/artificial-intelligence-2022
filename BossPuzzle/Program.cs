using BossPuzzle.Dao;
using BossPuzzle.PuzzleBoard;
using BossPuzzle.Utils;
using System.Diagnostics;
using System.Text;

namespace BossPuzzle;
using Dir = Board.Direction;
using Heur = AStar.Heuristic;

class Program
{
    public static void Main(string[] args)
    {
        Global.EnsureDirectoryIsValid();

        if (args.Length == 0)
        {
            TestRun();
        }
        else
        {
            ActualRun(args);
        }
    }

    private static void ActualRun(string[] args)
    {
        (var board, var solver, var solutionWritter, var additionalWritter) = InputParser(args);

        board.Print();

        var solvedBoard = board.Solve(solver);

        if (solvedBoard.IsValid()) Console.WriteLine($"SOLVED in {solver.GetTimeConsumed():n3}ms");
        solvedBoard.Print();

        string path = solvedBoard.GetPathFormatted();
        Console.WriteLine($"Created {Board.instances:n0} instances of Board.");
        Console.WriteLine($"Path: {path} (length = {path.Length})");

        var additionalInfo = FileWriter.PrepareAdditionalInfo(solvedBoard, solver);

        solutionWritter.Write(solvedBoard);
        additionalWritter.Write(additionalInfo);
    }

    private static void TestRun()
    {
        /*var readFile = new FileFifteenPuzzleDao("test.file");
        var board = readFile.Read();*/

        var board = PuzzleGenerator.Generate(4, 4, 18);
        board.Print();


        //IPuzzleSolver solver = new AStar(Heur.Manhattan);

        /*IPuzzleSolver solver = new BFS(new[]
        {
            Dir.Up,
            Dir.Down,
            Dir.Left,
            Dir.Right
        });*/

        DFS solver = new DFS(
            new[] {
            Dir.Up,
            Dir.Down,
            Dir.Left,
            Dir.Right
            },
            19);
        
        var solvedBoard = board.Solve(solver);

        if (solvedBoard.IsValid()) Console.WriteLine($"SOLVED in {solver.GetTimeConsumed():n3}ms");
        solvedBoard.Print();

        string path = solvedBoard.GetPathFormatted();
        Console.WriteLine($"Created {solver.GetBoardInstanceCount():n0} instances of Board.");
        Console.WriteLine($"Path: {path} (length = {path.Length})");

        /*var saveFile = new FileFifteenPuzzleDao("test_sol.file");
        saveFile.Write(solvedBoard);*/

        Console.ReadKey();
    }

    private static (Board board, IPuzzleSolver solver, FileFifteenPuzzleDao solutionWritter, FileWriter additionalWritter) InputParser(string[] args)
    {
        if (args is null || args.Length != 5) throw new ArgumentException("Argument list must contain 5 arguments!", nameof(args));

        string strategy = args[0];
        string specs = args[1];
        string boardFile = args[2];
        string solutionFile = args[3];
        string additionalFile = args[4];

        Board board;
        IPuzzleSolver solver;
        FileFifteenPuzzleDao solutionWritter;
        FileWriter additionalWritter;

        solver = strategy switch
        {
            "bfs" => new BFS(StringToDirections(specs.ToUpper())),
            "dfs" => new DFS(StringToDirections(specs.ToUpper())),
            "astr" => specs.ToLower() switch
            {
                "hamm" => new AStar(Heur.Hamming),
                "manh" => new AStar(Heur.Manhattan),
                _ => throw new ArgumentException($"Heuristic '{specs}' not recognized", nameof(specs)),
            },
            _ => throw new ArgumentException($"Algorithm '{strategy}' not recognized", nameof(strategy)),
        };

        var readFile = new FileFifteenPuzzleDao(boardFile);
        board = readFile.Read();
        solutionWritter = new FileFifteenPuzzleDao(solutionFile);
        additionalWritter = new FileWriter(additionalFile);

        return (board, solver, solutionWritter, additionalWritter);
    }

    private static Dir[] StringToDirections(string spec)
    {
        if(spec is null || spec.Length != 4) throw new ArgumentException("Direction specification must contain 4 characters", nameof(spec));

        var dirs = new Dir[4];
        for (int i = 0; i < 4; i++)
        {
            Dir direction;
            switch (spec[i])
            {
                case 'R':
                    direction = Dir.Right;
                    break;
                case 'L':
                    direction = Dir.Left;
                    break;
                case 'U':
                    direction = Dir.Up;
                    break;
                case 'D':
                    direction = Dir.Down;
                    break;
                default:
                    throw new ArgumentException($"Direction {spec[i]} not recignized!");
            }
            dirs[i] = direction;
        }

        var set = new HashSet<Dir>(dirs);
        if (set.Count != 4) throw new ArgumentException("Direction specification must contain 4 DIFFERENT characters");

        return dirs;
    }
}