namespace BossPuzzle.PuzzleBoard;

public class Hammings : IPuzzleSolver
{
    private readonly int _maxTries;

    public Hammings(int maxTries)
    { 
        _maxTries = maxTries;
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

                if (prevNextBoard.Hammings > nextBoard.Hammings)
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