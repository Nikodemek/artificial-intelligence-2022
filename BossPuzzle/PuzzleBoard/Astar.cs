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
        if (board.IsValid()) return board;

        var currBoard = board;

        int tries = 0;
        while (!currBoard.IsValid())
        {
            tries++;

            var dists = currBoard.ClarifyMovement();
            int distLength = dists.Length;

            int currDistance = 0; 
            switch (_atype)
            {
                case Atype.Hamming:
                    currDistance = currBoard.Hammings;
                    break;
                case Atype.Manhattan:
                    currDistance = currBoard.Manhattans;
                    break;
            }

            var possibleBoards = new Board[distLength];

            int minIndex = 0;
            for (var i = 0; i < distLength; i++)
            {
                var nextBoard = currBoard.Move(dists[i]);
                possibleBoards[i] = nextBoard;

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

                if (distance < currDistance) minIndex = i;
            }

            currBoard = possibleBoards[minIndex];

            if (tries > _maxTries) break;
        }

        Console.WriteLine($"Tries = {tries}");
        return currBoard;
    }

    public enum Atype
    {
        Hamming = 0,
        Manhattan = 1,
    }
}