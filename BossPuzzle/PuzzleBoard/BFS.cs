using BossPuzzle.Utils;

namespace BossPuzzle.PuzzleBoard;
using Dir = Board.Direction;


public class BFS: IPuzzleSolver
{
    private readonly Dir[] _directions;
    
    public BFS(Dir[] directions)
    {
        _directions = Cloner.SingleArr(directions);
    }

    public void Solve(Board board)
    {
        if (board.IsValid())
        {
            Console.WriteLine("Cleared.");
            return;
        }
        var visited = new HashSet<int> { board.GetHashCode() };
        var clearedDirections = board.ClarifyMovement(_directions);
        var queue = new List<Board>();
        foreach (var direction in clearedDirections)
        {
            var nextBoard = board.Move(direction);
            if (nextBoard.IsValid())
            {
                Console.WriteLine("Cleared.");
                return;
            }
            queue.Add(nextBoard);
        }

        while (queue.Count > 0)
        {
            var currentBoard = queue[0];
            queue.RemoveAt(0);
            clearedDirections = currentBoard.ClarifyMovement(_directions);
            foreach (var direction in clearedDirections)
            {
                var nextBoard = board.Move(direction);
                if (nextBoard.IsValid())
                {
                    Console.WriteLine("Cleared.");
                    return;
                }
                var hashedBoard = board.GetHashCode();
                if (!visited.Contains(hashedBoard)){
                    visited.Add(hashedBoard);
                    queue.Add(nextBoard);
                }
            }
        }
    }
}