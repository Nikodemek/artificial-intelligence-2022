using BossPuzzle.PuzzleBoard;

namespace BossPuzzle.Utils;
using Dir = Board.Direction;

public static class PuzzleGenerator
{
    public static Board Generate(short rowSize, short columnSize, int steps)
    {
        var board = new short[rowSize][];

        int size = rowSize * columnSize;
        for (var i = 0; i < rowSize; i++)
        {
            int offset = i * rowSize;
            board[i] = new short[columnSize];

            for (var j = 0; j < columnSize; j++)
            {
                board[i][j] = (short)((offset + j + 1) % size);
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

            ref short changedCell = ref board[0][0];
            ref short originalCell = ref board[emptyRow][emptyColumn];
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