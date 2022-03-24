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

        var set = new HashSet<ulong>();
        var stack = new Stack<Board>();

        set.Add(currBoard.Hash);

        int tries = 0;
        while (!currBoard.IsValid())
        {
            var dists = currBoard.ClarifyMovement();
            int distsLength = dists.Length;

            uint currDistance = GetDistance(currBoard);
            var possibleBoards = new (Board board, uint dist)[distsLength];

            for (var i = 0; i < distsLength; i++)
            {
                var nextBoard = currBoard.Move(dists[i]);
                uint distance = GetDistance(nextBoard);
                possibleBoards[i] = (nextBoard, distance);
            }

            Array.Sort(possibleBoards, (a, b) => a.dist.CompareTo(b.dist));

            foreach (var possibleBoard in possibleBoards)
            {
                uint nextBoardDist = possibleBoard.dist;
                if (nextBoardDist >= currDistance) continue;

                var nextboard = possibleBoard.board;
                if (set.Add(nextboard.Hash))
                {
                    stack.Push(nextboard);
                    break;
                }
            }

            if (stack.Count <= 0) break;

            currBoard = stack.Pop();

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