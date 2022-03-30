using FifteenPuzzle.Dao;
using FifteenPuzzle.PuzzleBoard;

namespace FifteenPuzzle.Utils;
using Dir = Board.Direction;

public static class PuzzleGenerator
{
    public static Board Generate(short rowSize, short columnSize, int steps)
    {
        var board = new short[rowSize, columnSize];

        int size = rowSize * columnSize;
        for (var i = 0; i < rowSize; i++)
        {
            int offset = i * rowSize;
            for (var j = 0; j < columnSize; j++)
            {
                board[i, j] = (short)((offset + j + 1) % size);
            }
        }

        int emptyRow = rowSize - 1;
        int emptyColumn = columnSize - 1;

        var rand = Random.Shared;
        Dir cancellingDir = Dir.Right;

        for (int i = 0; i < steps;)
        {
            Dir dir;
            do
            {
                dir = (Dir)rand.Next(4);
            } while (dir == cancellingDir);

            ref short changedCell = ref board[0, 0];
            ref short originalCell = ref board[emptyRow, emptyColumn];
            switch (dir)
            {
                case Dir.Up:
                    if (emptyRow <= 0) continue;
                    changedCell = ref board[emptyRow - 1, emptyColumn];
                    emptyRow--;
                    cancellingDir = Dir.Down;
                    break;
                case Dir.Down:
                    if (emptyRow >= columnSize - 1) continue;
                    changedCell = ref board[emptyRow + 1, emptyColumn];
                    emptyRow++;
                    cancellingDir = Dir.Up;
                    break;
                case Dir.Right:
                    if (emptyColumn >= rowSize - 1) continue;
                    changedCell = ref board[emptyRow, emptyColumn + 1];
                    emptyColumn++;
                    cancellingDir = Dir.Left;
                    break;
                case Dir.Left:
                    if (emptyColumn <= 0) continue;
                    changedCell = ref board[emptyRow, emptyColumn - 1];
                    emptyColumn--;
                    cancellingDir = Dir.Right;
                    break;
                default:
                    continue;
            }
            (changedCell, originalCell) = (originalCell, changedCell);

            i++;
        }

        return new Board(board);
    }
    
    public static void GenerateAll(uint maxDepth)
    {
        var boards = new List<Board>();
        Dir[] exampleDirections =
        {
            Dir.Up,
            Dir.Down,
            Dir.Left,
            Dir.Right
        };

        var boardsVisited = new HashSet<ulong>();
        var boardsQueue = new Queue<Board>();

        var currentBoard = Generate(4, 4, 0);

        boardsVisited.Add(currentBoard.Hash);
        boardsQueue.Enqueue(currentBoard);

        while (boardsQueue.Count > 0)
        {
            currentBoard = boardsQueue.Dequeue();
            var directions = currentBoard.ClarifyMovement(exampleDirections);

            var evacuationFlag = false;
            foreach (var direction in directions)
            {
                var nextBoard = currentBoard.Move(direction);
                if (nextBoard.GetPathLength() > maxDepth)
                {
                    evacuationFlag = true;
                    break;
                }
                if (boardsVisited.Add(nextBoard.Hash))
                {
                    boardsQueue.Enqueue(nextBoard);
                    boards.Add(nextBoard);
                }
            }

            if (evacuationFlag) break;
        }

        var bw = new BoardWriter();
        bw.Write(boards);
    }
}