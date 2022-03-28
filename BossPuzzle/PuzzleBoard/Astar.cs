using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BossPuzzle.PuzzleBoard;

public class AStar : IPuzzleSolver
{
    private readonly Heuristic _atype;
    
    private uint _boardInstanceCount;
    private uint _maxDepthAchieved;
    private readonly Stopwatch _stoper;
    
    public AStar(Heuristic atype)
    {
        _atype = atype;
        _boardInstanceCount = 1;
        _maxDepthAchieved = 0;
        _stoper = new Stopwatch();
    }

    public Board Solve(in Board board)
    {
        _boardInstanceCount = 1;
        _maxDepthAchieved = 0;
        _stoper.Start();

        var queue = new PriorityQueue<Board, uint>();

        var currBoard = board;

        while (!currBoard.IsValid())
        {
            var directions = currBoard.ClarifyMovement();
            foreach (var direction in directions)
            {
                var nextBoard = currBoard.Move(direction);
                _boardInstanceCount++;
                uint pathDist = (uint)nextBoard.GetPathLength();
                uint heurDist = GetHeuristicDistance(nextBoard);
                queue.Enqueue(nextBoard, pathDist + heurDist);
            }
            currBoard = queue.Dequeue();
        }
        
        _stoper.Stop();

        return currBoard;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private uint GetHeuristicDistance(in Board board)
    {
        return _atype switch
        {
            Heuristic.Hamming => board.DistanceHammings,
            Heuristic.Manhattan => board.DistanceManhattan,
            _ => throw new ArgumentOutOfRangeException(nameof(_atype), "Atype not recognized."),
        };
    }

    public enum Heuristic
    {
        Hamming = 0,
        Manhattan = 1,
    }
}