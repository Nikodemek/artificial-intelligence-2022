using BossPuzzle.Utils;

namespace BossPuzzle.PuzzleBoard;
using Dir = Board.Direction;

public class DFS : IPuzzleSolver
{
    private readonly int _maxDepth;
    private readonly Dir[] _directions;

    public DFS(Dir[] directions)
        : this(directions, 7)
    { }

    public DFS(Dir[] directions, int maxDepth)
    {
        _maxDepth = maxDepth;
        _directions = Arrayer.Copy(directions).Reverse();
    }

    public Board Solve(in Board board)
    {
        if (board.IsValid()) return board;

        var visited = new HashSet<ulong>();
        var stack = new Stack<Board>();

        stack.Push(board);
        var currentBoard = board;

        while (stack.Count > 0)
        {
            while (stack.Count > _maxDepth)
            {
                currentBoard = stack.Pop();
            }
            
            visited.Add(currentBoard.Hash);

            var directions = currentBoard.ClarifyMovement(_directions);

            bool flag = false;
            foreach (var direction in directions)
            {
                var nextBoard = currentBoard.Move(direction);

                if (nextBoard.IsValid()) return nextBoard;

                if (!visited.Contains(nextBoard.Hash))
                {
                    flag = true;
                    stack.Push(nextBoard);
                    currentBoard = nextBoard;
                    break;
                }
            }

            if (!flag)
            {
                currentBoard = stack.Pop();
            }
        }

        return board;
    }
    
    /*
    public Board Solve(in Board board)
    {
        if (board.IsValid()/* || _recursionDepth > _maxDepth#1#) return board;
        
        /*_recursionDepth++;#1#

        visited.Add(board.Hash);

        var directions = board.ClarifyMovement(_directions);

        foreach (var direction in directions)
        {
            var nextBoard = board.Move(direction);
        
            if (nextBoard.IsValid()) return nextBoard;

            if (!visited.Contains(nextBoard.Hash)) return Solve(nextBoard);
        }
        
        return board;
    }
    */
    
}