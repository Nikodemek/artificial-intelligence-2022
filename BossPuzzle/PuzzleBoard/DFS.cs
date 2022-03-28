using System.Diagnostics;
using BossPuzzle.Utils;

namespace BossPuzzle.PuzzleBoard;
using Dir = Board.Direction;

public class DFS : IPuzzleSolver
{
    private readonly int _maxDepth;
    private readonly Dir[] _directions;

    private uint _boardInstanceCount;
    private uint _maxDepthAchieved;
    private readonly Stopwatch _stoper;

    public DFS(Dir[] directions)
        : this(directions, 19)
    { }

    public DFS(Dir[] directions, int maxDepth)
    {
        _maxDepth = maxDepth;
        _directions = Arrayer.Copy(directions);
        _boardInstanceCount = 1;
        _maxDepthAchieved = 0;
        _stoper = new Stopwatch();
    }

    public Board Solve(in Board board)
    {
        _boardInstanceCount = 1;
        _maxDepthAchieved = 0;
        
        if (board is null) throw new ArgumentNullException(nameof(board));
        
        _stoper.Start();

        var stack = new Stack<Board>();
        var boardsDepth = new Dictionary<ulong, short>();
        var validBoards = new List<Board>();

        var currentBoard = board;
        stack.Push(currentBoard);

        while (stack.Count > 0)
        {
            if (stack.Count > _maxDepth)
            {
                stack.Pop();
                currentBoard = stack.Peek();
            }
            
            if (_maxDepthAchieved < stack.Count)
            {
                _maxDepthAchieved = (uint) stack.Count;
            }
            
            if (currentBoard.IsValid()) validBoards.Add(currentBoard);

            bool boardAdded = false;
            var directions = currentBoard.ClarifyMovement(_directions);
            foreach (var direction in directions)
            {
                var nextBoard = currentBoard.Move(direction);
                _boardInstanceCount++;
                
                short stackCount = (short)stack.Count;
                ulong nextBoardHash = nextBoard.Hash;

                if (!boardsDepth.TryAdd(nextBoardHash, stackCount))
                {
                    if (stackCount >= boardsDepth[nextBoardHash]) continue;
                    boardsDepth[nextBoardHash] = stackCount;
                }

                boardAdded = true;
                stack.Push(nextBoard);
                currentBoard = nextBoard;
                break;
            }

            if (!boardAdded)
            {
                stack.Pop();
                stack.TryPeek(out currentBoard!);
            }
        }

        if (validBoards.Count == 0) return board;
        
        validBoards.Sort((board1, board2) => board1.GetPathLength().CompareTo(board2.GetPathLength()));

        _stoper.Stop();
        
        return validBoards[0];
    }

    public uint GetBoardInstanceCount()
    {
        return _boardInstanceCount;
    }

    public uint GetMaxDepthAchieved()
    {
        return _maxDepthAchieved;
    }

    public double GetTimeConsumed()
    {
        return _stoper.Elapsed.TotalMilliseconds;
    }
}