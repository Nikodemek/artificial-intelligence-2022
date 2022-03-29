using FifteenPuzzle.Utils;
using System.Diagnostics;

namespace FifteenPuzzle.PuzzleBoard;
using Dir = Board.Direction;

public class DFS : IPuzzleSolver, IPuzzleSolverDiagnostics
{
    private readonly int _maxDepth;
    private readonly Dir[] _directions;

    public DFS(Dir[] directions)
        : this(directions, 19)
    { }

    public DFS(Dir[] directions, int maxDepth)
    {
        _directions = Arrayer.Copy(directions);
        _maxDepth = maxDepth;
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
        maxDepth = _maxDepth;
        var watch = Stopwatch.StartNew();

        var boardsStack = new Stack<Board>();
        var boardsDepth = new Dictionary<ulong, short>();
        var validBoards = new List<Board>();

        var currentBoard = board;
        boardsStack.Push(currentBoard);

        while (boardsStack.Count > 0)
        {
            if (boardsStack.Count > _maxDepth)
            {
                boardsStack.Pop();
                currentBoard = boardsStack.Peek();
            }

            if (currentBoard.IsValid()) validBoards.Add(currentBoard);

            bool boardAdded = false;
            var directions = currentBoard.ClarifyMovement(_directions);
            foreach (var direction in directions)
            {
                var nextBoard = currentBoard.Move(direction);
                visited++;

                short stackCount = (short)boardsStack.Count;
                ulong nextBoardHash = nextBoard.Hash;

                if (!boardsDepth.TryAdd(nextBoardHash, stackCount))
                {
                    if (stackCount >= boardsDepth[nextBoardHash]) continue;
                    boardsDepth[nextBoardHash] = stackCount;
                }

                boardAdded = true;
                boardsStack.Push(nextBoard);
                currentBoard = nextBoard;
                break;
            }

            if (!boardAdded)
            {
                boardsStack.Pop();
                boardsStack.TryPeek(out currentBoard!);
            }
            processed++;
        }

        validBoards.Sort((board1, board2) => board1.GetPathLength().CompareTo(board2.GetPathLength()));

        watch.Stop();
        time = watch.Elapsed.TotalMilliseconds;

        return validBoards.Count > 0 ? validBoards[0] : board;
    }
}