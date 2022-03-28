using System.Diagnostics;
using BossPuzzle.Utils;

namespace BossPuzzle.PuzzleBoard;
using Dir = Board.Direction;

public class BFS : IPuzzleSolver
{
    private readonly Dir[] _directions;

    private uint _boardInstanceCount;
    private uint _maxDepthAchieved;
    private readonly Stopwatch _stoper;
    
    public BFS(Dir[] directions)
    {
        _directions = Arrayer.Copy(directions);
        _boardInstanceCount = 1;
        _maxDepthAchieved = 0;
        _stoper = new Stopwatch();
    }

    public Board Solve(in Board board)
    {
        _stoper.Start();

        if (board.IsValid()) return board;

        var visited = new HashSet<ulong>();
        var queue = new Queue<Board>();

        visited.Add(board.Hash);
        queue.Enqueue(board);

        while (queue.Count > 0)
        {
            var currentBoard = queue.Dequeue();
            var directions = currentBoard.ClarifyMovement(_directions);

            foreach (var direction in directions)
            {
                var nextBoard = currentBoard.Move(direction);
                _boardInstanceCount++;

                if (nextBoard.IsValid())
                {
                    var depth = nextBoard.GetPath().Length;
                    _maxDepthAchieved = (uint) depth;
                    return nextBoard;
                }

                if (visited.Add(nextBoard.Hash)) queue.Enqueue(nextBoard);
            }
        }

        _stoper.Stop();
        
        return board;
    }

    public uint GetBoardInstanceCount()
    {
        return _boardInstanceCount;
    }

    public uint GetMaxDepthAchieved()
    {
        return _maxDepthAchieved;
    }

    public double GetTimeConsumed()
    {
        return _stoper.Elapsed.TotalMilliseconds;
    }
}