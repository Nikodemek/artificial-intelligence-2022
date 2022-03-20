using BossPuzzle.Utils;

namespace BossPuzzle.PuzzleBoard;
using Dir = Board.Direction;

public class DFS : IPuzzleSolver
{
    private readonly int _depth;
    private readonly Dir[] _directions;
    private HashSet<ulong> visited;
    private int _recursionDepth;

    public DFS(Dir[] directions)
        : this(directions, 20)
    { }

    public DFS(Dir[] directions, int depth)
    {
        _directions = Cloner.SingleArr(directions)
                            .Reverse()
                            .ToArray();
        _depth = depth;
        visited = new HashSet<ulong>();
        _recursionDepth = 0;
    }

    /*public Board Solve(in Board board)
    {
        if (board.IsValid()) return board;

        var visited = new HashSet<ulong>();
        var stack = new Stack<Board>();
        var recursionDepth = 0;

        stack.Push(board);

        while (stack.Count > 0)
        {
            Console.WriteLine(recursionDepth);
            Console.WriteLine(stack.Count);

            var currentBoard = stack.Pop();
            
            // this is the only place that should define board as visited
            if (!visited.Add(currentBoard.Hash))
            {
                recursionDepth--;
                continue;
            }

            if (recursionDepth > _depth)
            {
                continue;
            }

            var directions = currentBoard.ClarifyMovement(_directions);

            foreach (var direction in directions)
            {
                var nextBoard = currentBoard.Move(direction);
                nextBoard.AddToPath(direction);
            
                if (nextBoard.IsValid()) return nextBoard;
            
                if (!visited.Contains(nextBoard.Hash)) stack.Push(nextBoard);
            }

        }

        return board;
    }*/
    
    public Board Solve(in Board board)
    {
        if (board.IsValid() || _recursionDepth > _depth) return board;
        
        _recursionDepth++;

        visited.Add(board.Hash);

        var directions = board.ClarifyMovement(_directions);

        foreach (var direction in directions)
        {
            var nextBoard = board.Move(direction);
            nextBoard.AddToPath(direction);
        
            if (nextBoard.IsValid()) return nextBoard;

            if (!visited.Contains(nextBoard.Hash)) return Solve(nextBoard);
        }
        
        return board;
    }
    
}