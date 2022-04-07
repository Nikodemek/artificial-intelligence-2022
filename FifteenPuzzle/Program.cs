using FifteenPuzzle.Dao;
using FifteenPuzzle.PuzzleBoard;
using FifteenPuzzle.Utils;

namespace FifteenPuzzle;
using Dir = Board.Direction;
using Heur = AStar.Heuristic;

class Program
{
    public static void Main(string[] args)
    {
        Global.EnsureDirectoryIsValid(false);

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
        var (board, solver, solutionWriter, additionalWriter) = InputParser(args);

        board.Print();

        var solvedBoard = board.Solve(solver, out RunInfo runInfo);

        solvedBoard.Print();
        Console.WriteLine(runInfo);

        solutionWriter.Write(runInfo);
        additionalWriter.Write(runInfo);

        Console.WriteLine($"All data written to {Global.BaseDataDirPath}");
    }

    private static void TestRun()
    {
        var readFile = new FileFifteenReader("4x4_07_0255.txt");
        //var board = readFile.Read();


        //var board = PuzzleGenerator.Generate(4, 4, 0);
        //PuzzleGenerator.GenerateAll(7);
        short[,] game = {
            {2, 3, 4, 0},
            {6, 7, 12, 8},
            {1, 5, 10, 11 },
            {9, 13, 14, 15 } 
        };
        var board = new Board(game);
        board.Print();


        IPuzzleSolverDiagnostics solver = new AStar(Heur.Hamming);

        /*IPuzzleSolverDiagnostics solver = new BFS(new[]
        {
            Dir.Up,
            Dir.Down,
            Dir.Left,
            Dir.Right
        });*/

        /*IPuzzleSolverDiagnostics solver = new DFS(
            new[] {
            Dir.Up,
            Dir.Down,
            Dir.Left,
            Dir.Right
            },
            20);*/

        var solvedBoard = board.Solve(solver, out RunInfo runInfo);

        solvedBoard.Print();
        Console.WriteLine(runInfo);

        /*var saveFile = new FileFifteenPuzzleDao("test_sol.file");
        saveFile.Write(solvedBoard);*/

        Console.ReadKey();
    }

    private static (Board board, IPuzzleSolverDiagnostics solver, BasicInfoWriter solutionWritter, AdditionalInfoWriter additionalWritter) InputParser(string[] args)
    {
        if (args is null || args.Length != 5) throw new ArgumentException("Argument list must contain 5 arguments!", nameof(args));

        string strategy = args[0];
        string specs = args[1];
        string boardFile = args[2];
        string solutionFile = args[3];
        string additionalFile = args[4];

        Board board;
        IPuzzleSolverDiagnostics solver;
        BasicInfoWriter solutionWriter;
        AdditionalInfoWriter additionalWriter;

        solver = strategy switch
        {
            "bfs" => new BFS(StringToDirections(specs.ToUpper())),
            "dfs" => new DFS(StringToDirections(specs.ToUpper())),
            "astr" => specs.ToLower() switch
            {
                "hamm" => new AStar(Heur.Hamming),
                "manh" => new AStar(Heur.Manhattan),
                _ => throw new ArgumentException($"Heuristic '{specs}' not recognized"),
            },
            _ => throw new ArgumentException($"Algorithm '{strategy}' not recognized"),
        };

        var readFile = new FileFifteenReader(boardFile);
        board = readFile.Read();
        solutionWriter = new BasicInfoWriter(solutionFile);
        additionalWriter = new AdditionalInfoWriter(additionalFile);

        return (board, solver, solutionWriter, additionalWriter);
    }

    private static Dir[] StringToDirections(string spec)
    {
        if (spec is null || spec.Length != 4) throw new ArgumentException("Direction specification must contain 4 characters", nameof(spec));

        var dirs = new Dir[4];
        for (var i = 0; i < 4; i++)
        {
            dirs[i] = spec[i] switch
            {
                'R' => Dir.Right,
                'L' => Dir.Left,
                'U' => Dir.Up,
                'D' => Dir.Down,
                _ => throw new ArgumentException($"Direction {spec[i]} not recignized!"),
            };
        }

        var set = new HashSet<Dir>(dirs);
        if (set.Count != 4) throw new ArgumentException("Direction specification must contain 4 DIFFERENT characters");

        return dirs;
    }
}