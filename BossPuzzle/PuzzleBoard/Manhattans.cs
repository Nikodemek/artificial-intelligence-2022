namespace BossPuzzle.PuzzleBoard;

public class Manhattans : IPuzzleSolver
{
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

                if (prevNextBoard.Manhattans > nextBoard.Manhattans)
                {
                    prevNextBoard = nextBoard;
                }
            }
            currBoard = prevNextBoard;

            if (tries > 30) break;
        }

        Console.WriteLine($"Tries = {tries}");
        return currBoard;
    }
}