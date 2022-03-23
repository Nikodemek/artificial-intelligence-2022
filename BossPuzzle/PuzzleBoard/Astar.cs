namespace BossPuzzle.PuzzleBoard;

public class Astar : IPuzzleSolver
{
    private readonly int _maxTries;
    private readonly Atype _atype;

    public Astar(Atype atype, int maxTries)
    {
        _maxTries = maxTries;
        _atype = atype;
    }

    public Astar(Atype atype)
        : this(atype, 10_000)
    { }



    public enum Atype
    {
        Hamming = 0,
        Manhattan = 1,
    }

    public Board Solve(in Board board)
    {
        if (board.IsValid()) return board;

        var currBoard = board;

        int tries = 0;
        while (!currBoard.IsValid())
        {
            tries++;

            var dists = currBoard.ClarifyMovement();
            int distLength = dists.Length;

            var possibilities = new Board[distLength];

            int minIndex = 0;
            for (var i = 0; i < distLength; i++)
            {
                var nextBoard = currBoard.Move(dists[i]);
                possibilities[i] = nextBoard;

                int distance = 0;
                switch (_atype)
                {
                    case Atype.Hamming:
                        distance = nextBoard.Hammings;
                        break;
                    case Atype.Manhattan:
                        distance = nextBoard.Manhattans;
                        break;
                }

                if (distance < minIndex) minIndex = i;
            }

            currBoard = possibilities[minIndex];

            if (tries > _maxTries) break;
        }

        Console.WriteLine($"Tries = {tries}");
        return currBoard;
    }
}