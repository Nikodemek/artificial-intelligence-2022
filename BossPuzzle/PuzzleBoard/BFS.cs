using BossPuzzle.Utils;
using System.Collections.Generic;

namespace BossPuzzle.PuzzleBoard;
using Dir = Board.Direction;

public class BFS: IPuzzleSolver
{
    private readonly Dir[] _directions;
    
    public BFS(Dir[] directions)
    {
        _directions = Cloner.SingleArr(directions);
    }

    public Board Solve(Board board)
    {
        if (board.IsValid()) return board;

        var visited = new HashSet<ulong>();
        var queue = new Queue<Board>();

        visited.Add(board.Hash); 
        queue.Enqueue(board);

        while (queue.Count > 0)
        {
            var currentBoard = queue.Dequeue();
            var directions = currentBoard.ClarifyMovement(_directions);

            foreach (var direction in directions)
            {
                var nextBoard = currentBoard.Move(direction);

                if (nextBoard.IsValid()) return nextBoard;

                if (visited.Add(nextBoard.Hash))
                {
                    queue.Enqueue(nextBoard);
                }
            }
        }

        return board;
    }
}