using BossPuzzle.PuzzleBoard;

namespace BossPuzzle.Utils;
using Dir = Board.Direction;

public static class PuzzleGenerator
{
    public static Board Generate(long rowSize, long columnSize, int steps)
    {
        var board = new long[rowSize][];

        long size = rowSize * columnSize;
        for (var i = 0; i < rowSize; i++)
        {
            long offset = i * rowSize;
            board[i] = new long[columnSize];

            for (var j = 0; j < columnSize; j++)
            {
                board[i][j] = (long)((offset + j + 1) % size);
            }
        }

        long emptyRow = rowSize - 1;
        long emptyColumn = columnSize - 1;

        var rand = Random.Shared;
        Dir cancellingDir = Dir.Right;

        for (int i = 0; i < steps;)
        {
            Dir dir;
            do
            {
                dir = (Dir)rand.Next(4);
            } while (dir == cancellingDir);

            ref long changedCell = ref board[0][0];
            ref long originalCell = ref board[emptyRow][emptyColumn];
            switch (dir)
            {
                case Dir.Up:
                    if (emptyRow <= 0) continue;
                    changedCell = ref board[emptyRow - 1][emptyColumn];
                    emptyRow--;
                    cancellingDir = Dir.Down;
                    break;
                case Dir.Down:
                    if (emptyRow >= columnSize - 1) continue;
                    changedCell = ref board[emptyRow + 1][emptyColumn];
                    emptyRow++;
                    cancellingDir = Dir.Up;
                    break;
                case Dir.Right:
                    if (emptyColumn >= rowSize - 1) continue;
                    changedCell = ref board[emptyRow][emptyColumn + 1];
                    emptyColumn++;
                    cancellingDir = Dir.Left;
                    break;
                case Dir.Left:
                    if (emptyColumn <= 0) continue;
                    changedCell = ref board[emptyRow][emptyColumn - 1];
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
}