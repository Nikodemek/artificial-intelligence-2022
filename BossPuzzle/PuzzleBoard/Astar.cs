using System.Runtime.CompilerServices;

namespace BossPuzzle.PuzzleBoard;

public class Astar : IPuzzleSolver
{
    private readonly int _maxTries;
    private readonly Atype _atype;

    public Astar(Atype atype)
        : this(atype, 10_000)
    { }

    public Astar(Atype atype, int maxTries)
    {
        _maxTries = maxTries;
        _atype = atype;
    }

    public Board Solve(in Board board)
    {
        var currBoard = board;

        var visited = new HashSet<ulong>();
        var stack = new Stack<Board>();

        stack.Push(currBoard);

        int tries = 0;
        while (stack.Count > 0)
        {
            currBoard = stack.Pop();
            
            if (!visited.Add(currBoard.Hash)) continue;
            
            var directions = currBoard.ClarifyMovement();
            int directionsLength = directions.Length;

            uint currDistance = GetDistance(currBoard);
            var possibleBoards = new (Board board, uint dist)[directionsLength];

            for (var i = 0; i < directionsLength; i++)
            {
                var nextBoard = currBoard.Move(directions[i]);
                uint distance = GetDistance(nextBoard);
                possibleBoards[i] = (nextBoard, distance);
            }

            Array.Sort(possibleBoards, (a, b) => a.dist.CompareTo(b.dist));

            foreach (var possibleBoard in possibleBoards)
            {
                uint nextBoardDist = possibleBoard.dist;
                if (nextBoardDist >= currDistance) continue;

                var nextboard = possibleBoard.board;
                if (!visited.Contains(nextboard.Hash)) stack.Push(nextboard);
            }

            if (++tries >= _maxTries) break;
        }

        Console.WriteLine($"Tries = {tries}");
        return currBoard;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private uint GetDistance(in Board board)
    {
        return _atype switch
        {
            Atype.Hamming => board.Hammings,
            Atype.Manhattan => board.Manhattans,
            _ => throw new ArgumentOutOfRangeException(nameof(_atype), "Atype not recognized."),
        };
    }

    public enum Atype
    {
        Hamming = 0,
        Manhattan = 1,
    }
}