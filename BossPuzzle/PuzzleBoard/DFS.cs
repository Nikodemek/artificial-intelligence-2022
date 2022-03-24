using System.Data.Common;
using BossPuzzle.Utils;

namespace BossPuzzle.PuzzleBoard;
using Dir = Board.Direction;

public class DFS : IPuzzleSolver
{
    private readonly int _maxDepth;
    private readonly Dir[] _directions;

    public DFS(Dir[] directions)
        : this(directions, 21)
    { }

    public DFS(Dir[] directions, int maxDepth)
    {
        _maxDepth = maxDepth;
        _directions = Arrayer.Copy(directions);
    }

    public Board Solve(in Board board)
    {
        var stack = new Stack<Board>();
        var visitedExtend = new Dictionary<ulong, short>();
        var validBoards = new List<Board>();

        if (board.IsValid()) return board;

        stack.Push(board);
        var currentBoard = board;

        while (stack.Count > 0)
        {
            while (stack.Count > _maxDepth)
            {
                stack.Pop();
                currentBoard = stack.Pop();
                stack.Push(currentBoard);
            }

            var directions = currentBoard.ClarifyMovement(_directions);

            bool flag = false;
            foreach (var direction in directions)
            {
                var nextBoard = currentBoard.Move(direction);

                if (nextBoard.IsValid())
                {
                    validBoards.Add(nextBoard);
                    break;
                }
                
                if (!visitedExtend.TryAdd(nextBoard.Hash, (short)stack.Count))
                {
                    if (visitedExtend.TryGetValue(nextBoard.Hash, out var value))
                    {
                        if (value > stack.Count)
                        {
                            flag = true;
                            visitedExtend[nextBoard.Hash] = (short)stack.Count;
                            stack.Push(nextBoard);
                            currentBoard = nextBoard;
                            break;
                        }
                        continue;
                    }
                }

                flag = true;
                stack.Push(nextBoard);
                currentBoard = nextBoard;
                break;
            }

            if (!flag)
            {
                stack.Pop();
                if (stack.Count == 0)
                {
                    break;
                }
                currentBoard = stack.Pop();
                stack.Push(currentBoard);
            }
        }
        
        Array.Sort(validBoards.ToArray(), 
            (board1, board2) => board1.GetPath().Length.CompareTo(board2.GetPath().Length));

        return validBoards.First();
    }
}