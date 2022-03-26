using System.Runtime.CompilerServices;

namespace BossPuzzle.PuzzleBoard;

public class AStar : IPuzzleSolver
{
    private readonly int _maxTries;
    private readonly Heuristic _atype;

    public AStar(Heuristic atype)
        : this(atype, 10_000)
    { }

    public AStar(Heuristic atype, int maxTries)
    {
        _maxTries = maxTries;
        _atype = atype;
    }

    public Board Solve(in Board board)
    {
        var queue = new PriorityQueue<Board, uint>();

        var currBoard = board;

        while (!currBoard.IsValid())
        {
            var directions = currBoard.ClarifyMovement();
            foreach (var direction in directions)
            {
                var nextBoard = currBoard.Move(direction);
                uint pathDist = (uint)nextBoard.GetPathLength();
                uint heurDist = GetHeuristicDistance(nextBoard);
                queue.Enqueue(nextBoard, pathDist + heurDist);
            }
            currBoard = queue.Dequeue();
        }

        return currBoard;
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