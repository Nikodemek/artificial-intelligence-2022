using BossPuzzle.Utils;

namespace BossPuzzle.PuzzleBoard;
using Dir = Board.Direction;

public class DFS : IPuzzleSolver
{
    private readonly int _maxDepth;
    private readonly Dir[] _directions;
    /*private HashSet<ulong> visited;
    private int _recursionDepth;*/

    public DFS(Dir[] directions)
        : this(directions, 12)
    { }

    public DFS(Dir[] directions, int maxDepth)
    {
        _maxDepth = maxDepth;
        _directions = Arrayer.Copy(directions).Reverse();
        /*visited = new HashSet<ulong>();
        _recursionDepth = 0;*/
    }

    public Board Solve(in Board board)
    {
        if (board.IsValid()) return board;

        var visited = new HashSet<ulong>();
        var stack = new Stack<Board>();

        stack.Push(board);

        while (stack.Count > 0)
        {
            var currentBoard = stack.Pop();
            
            // this is the only place that should define board as visited
            if (!visited.Add(currentBoard.Hash)) continue;

            var directions = currentBoard.ClarifyMovement(_directions);
            
            foreach (var direction in directions)
            {
                var nextBoard = currentBoard.Move(direction);

                if (nextBoard.IsValid()) return nextBoard;
            
                if (!visited.Contains(nextBoard.Hash)) stack.Push(nextBoard);
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