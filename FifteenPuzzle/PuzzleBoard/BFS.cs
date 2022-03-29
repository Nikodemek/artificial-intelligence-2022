using FifteenPuzzle.Utils;
using System.Diagnostics;

namespace FifteenPuzzle.PuzzleBoard;
using Dir = Board.Direction;

public class BFS : IPuzzleSolver, IPuzzleSolverDiagnostics
{
    private readonly Dir[] _directions;

    public BFS(Dir[] directions)
    {
        _directions = Arrayer.Copy(directions);
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
        var watch = Stopwatch.StartNew();

        var boardsVisited = new HashSet<ulong>();
        var boardsQueue = new Queue<Board>();

        var currentBoard = board;

        boardsVisited.Add(currentBoard.Hash);
        boardsQueue.Enqueue(currentBoard);

        while (!currentBoard.IsValid() && boardsQueue.Count > 0)
        {
            currentBoard = boardsQueue.Dequeue();
            var directions = currentBoard.ClarifyMovement(_directions);

            foreach (var direction in directions)
            {
                var nextBoard = currentBoard.Move(direction);
                visited++;

                if (nextBoard.IsValid())
                {
                    currentBoard = nextBoard;
                    break;
                }

                if (boardsVisited.Add(nextBoard.Hash)) boardsQueue.Enqueue(nextBoard);
            }
            processed++;
        }

        watch.Stop();
        time = watch.Elapsed.TotalMilliseconds;
        maxDepth = currentBoard.GetPathLength();

        return currentBoard;
    }
}