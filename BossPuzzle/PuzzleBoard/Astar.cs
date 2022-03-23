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

            var prevNextBoard = currBoard;
            foreach (var dist in dists)
            {
                var nextBoard = currBoard.Move(dist);
                ulong prevDistance = 0; 
                ulong nextDistance = 0;
                switch (_atype)
                {
                    case Atype.Manhattan:
                        prevDistance = prevNextBoard.Manhattans;
                        nextDistance = nextBoard.Manhattans;
                        break;
                    case Atype.Hamming:
                        prevDistance = prevNextBoard.Manhattans;
                        nextDistance = nextBoard.Manhattans;
                        break;
                }

                if (prevDistance > nextDistance)
                {
                    prevNextBoard = nextBoard;
                }
            }
            currBoard = prevNextBoard;

            if (tries > _maxTries) break;
        }

        Console.WriteLine($"Tries = {tries}");
        return currBoard;
    }
}