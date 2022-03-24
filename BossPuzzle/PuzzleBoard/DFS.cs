using System.Data.Common;
using BossPuzzle.Utils;

namespace BossPuzzle.PuzzleBoard;
using Dir = Board.Direction;

public class DFS : IPuzzleSolver
{
    private readonly int _maxDepth;
    private readonly Dir[] _directions;

    public DFS(Dir[] directions)
        : this(directions, 3)
    { }

    public DFS(Dir[] directions, int maxDepth)
    {
        _maxDepth = maxDepth;
        _directions = Arrayer.Copy(directions);
    }

    public Board Solve(in Board board)
    {
        var visited = new HashSet<ulong>();
        var stack = new Stack<Board>();
        var visitedExtend = new Dictionary<ulong, short>();
        var validBoards = new List<Board>();
        
        if (board.IsValid()) validBoards.Add(board);

        stack.Push(board);
        var currentBoard = board;

        while (stack.Count > 0)
        {
            while (stack.Count > _maxDepth)
            {
                currentBoard = stack.Pop();
            }
            
            visited.Add(currentBoard.Hash);
            if (!visitedExtend.TryAdd(currentBoard.Hash, (short)stack.Count))
            {
                visitedExtend[currentBoard.Hash] = (short)stack.Count;
            }
            
            var directions = currentBoard.ClarifyMovement(_directions);

            bool flag = false;
            foreach (var direction in directions)
            {
                var nextBoard = currentBoard.Move(direction);

                if (nextBoard.IsValid())
                {
                    validBoards.Add(nextBoard);
                    
                }

                short value;
                if (visitedExtend.TryGetValue(nextBoard.Hash, out value))
                {
                    if (value > stack.Count)
                    {
                        flag = true;
                        stack.Push(nextBoard);
                        visitedExtend[nextBoard.Hash] = (short)stack.Count;
                        currentBoard = nextBoard;
                        break;
                    }
                }
                else
                {
                    flag = true;
                    stack.Push(nextBoard);
                    visitedExtend.Add(nextBoard.Hash, (short)stack.Count);
                    currentBoard = nextBoard;
                    break;
                }
            }

            if (!flag)
            {
                currentBoard = stack.Pop();
            }
        }
        
        Array.Sort(validBoards.ToArray(), 
            (board1, board2) => board1.GetPath().Length.CompareTo(board2.GetPath().Length));
        for (int i = 0; i < validBoards.Count - 1; i++)
        {
            var referenceEquals = Object.ReferenceEquals(validBoards[i], validBoards[i + 1]);
            Console.WriteLine(referenceEquals);
            /*foreach (var direction in validBoard.GetPath())
            {
                switch (direction)
                {
                    case Dir.Up:
                        Console.Write("U");
                        break;
                    case Dir.Down:
                        Console.Write("D");
                        break;
                    case Dir.Left:
                        Console.Write("L");
                        break;
                    case Dir.Right:
                        Console.Write("R");
                        break;
                }
            }

            Console.WriteLine();*/
        }
        
        return validBoards.First();
    }
}