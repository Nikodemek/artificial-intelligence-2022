using BossPuzzle.Utils;

namespace BossPuzzle.PuzzleBoard;
using Dir = Board.Direction;

public class DFS: IPuzzleSolver
{
    private readonly Dir[] _directions;
    
    public DFS(Dir[] directions)
    {
        _directions = Cloner.SingleArr(directions);
    }

    public Board Solve(Board board)
    {
        if (board.IsValid()) return board;

        var visited = new HashSet<ulong>();
        var stack = new Stack<Board>();

        stack.Push(board);

        while (stack.Count > 0)
        {
            if (stack.Count > 20)
            {
                stack.Pop();
            }
            
            var currentBoard = stack.Pop();
            visited.Add(board.Hash);
            var directions = currentBoard
                                    .ClarifyMovement(_directions)
                                    .Reverse()
                                    .ToArray();

            foreach (var direction in directions)
            {
                var nextBoard = currentBoard.Move(direction);
                nextBoard.AddToPath(direction);

                if (nextBoard.IsValid()) return nextBoard;

                if (visited.Add(nextBoard.Hash)) stack.Push(nextBoard);
            }
        }

        return board;
    }
}