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
        /*var visited = new HashSet<int> { board.GetHashCode() };
        var queue = new List<Dir> { board.ClarifyMovement(_directions) };

        while (queue.Count > 0)
        {
            var root = queue[0];
            queue.RemoveAt(0);
            var edges = board.Move(root);
            for (var edge : edges) {
                if (!visited.contains(edge)){
                    visited.add(edge);
                    queue.add(edge);
                }
            }
        }*/
    }
}