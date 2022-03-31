using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FifteenPuzzle.PuzzleBoard;

public class AStar : IPuzzleSolver, IPuzzleSolverDiagnostics
{
    private readonly Heuristic _heuristic;

    public AStar(Heuristic heuristic)
    {
        _heuristic = heuristic;
    }

    public Board Solve(in Board board, out RunInfo runInfo)
    {
        var solvedBoard = Solve(board, out int visited, out int processed, out int maxDepth, out double time);

        bool isSolutionValid = solvedBoard.IsValid();
        string path = solvedBoard.GetPathFormatted();
        int pathLength = path.Length;

        runInfo = new RunInfo(
            isSolutionValid,
            path,
            pathLength,
            visited,
            processed,
            maxDepth,
            time);

        return solvedBoard;
    }

    public Board Solve(in Board board)
    {
        return Solve(board, out _, out _, out _, out _);
    }

    private Board Solve(in Board board, out int visited, out int processed, out int maxDepth, out double time)
    {
        visited = 1;
        processed = 1;
        maxDepth = 0;
        var watch = Stopwatch.StartNew();

        var queue = new PriorityQueue<Board, uint>();

        var currBoard = board;

        while (!currBoard.IsValid())
        {
            processed++;
            var directions = currBoard.ClarifyMovement();
            foreach (var direction in directions)
            {
                var nextBoard = currBoard.Move(direction);
                visited++;
                uint pathDist = (uint)nextBoard.PathLength;
                uint heurDist = GetHeuristicDistance(nextBoard);
                queue.Enqueue(nextBoard, pathDist + heurDist);
            }
            maxDepth++;
            currBoard = queue.Dequeue();
        }

        watch.Stop();
        time = watch.Elapsed.TotalMilliseconds;

        return currBoard;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private uint GetHeuristicDistance(in Board board)
    {
        return _heuristic switch
        {
            Heuristic.Hamming => board.DistanceHammings,
            Heuristic.Manhattan => board.DistanceManhattan,
            _ => throw new ArgumentException($"Heuristic '{_heuristic}' not recognized."),
        };
    }

    public enum Heuristic
    {
        Hamming = 0,
        Manhattan = 1,
    }
}