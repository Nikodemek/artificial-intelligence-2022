using System;
using System.Text;
using BossPuzzle.Utils;

namespace BossPuzzle.PuzzleBoard;

public struct Board
{
    public int ColumnSize { get; init; }
    public int RowSize { get; init; }

    private readonly int[][] _board;

    private int emptyCellRow;
    private int emptyCellColumn;

    public Board(int[][] board)
        : this(board, true)
    { }

    private Board(int[][] board, bool copyArr)
        : this()
    {
        int columnLength = board.Length;

        if (columnLength <= 0) throw new ArgumentException("Board cannot be empty!");

        int rowLength = board[0].Length;

        if (rowLength <= 0) throw new ArgumentException("Rows cannot be empty!");

        if (copyArr)
        {
            this._board = new int[columnLength][];

            for (var i = 0; i < columnLength; i++)
            {
                int newRowLength = board[i].Length;

                if (newRowLength != rowLength) throw new ArgumentException("Rows sizes must be equal");

                rowLength = newRowLength;
                _board[i] = new int[rowLength];

                for (var j = 0; j < rowLength; j++)
                {
                    int value = board[i][j];
                    _board[i][j] = value;

                    if (value <= 0)
                    {
                        emptyCellRow = i;
                        emptyCellColumn = j;
                    }
                }
            }
        }
        else
        {
            this._board = board;

            for (var i = 0; i < columnLength; i++)
            {
                for (var j = 0; j < rowLength; j++)
                {
                    if (board[i][j] <= 0)
                    {
                        emptyCellRow = i;
                        emptyCellColumn = j;
                    }
                }
            }
        }

        RowSize = rowLength;
        ColumnSize = columnLength;
    }

    public bool IsValid()
    {
        int size = RowSize * ColumnSize;

        for (var i = 0; i < RowSize; i++)
        {
            int offset = i * RowSize;
            for (var j = 0; j < ColumnSize; j++)
            {
                bool valid = _board[i][j] == (offset + j + 1) % size;

                if (!valid) return false;
            }
        }
        return true;
    }

    public int At(int row, int column)
    {
        return _board[row][column];
    }

    // Used to move an empty cell
    public Board Move(Direction dir)
    {
        return Move(emptyCellRow, emptyCellColumn, dir);
    }

    // Used to move given cell
    public Board Move(int row, int column, Direction dir)
    {
        int[][] newBoard = Cloner.DoubleArr(_board);

        int temp;
        switch (dir)
        {
            case Direction.Up:
                if (row <= 0) break;
                temp = newBoard[row - 1][column];
                newBoard[row - 1][column] = _board[row][column];
                newBoard[row][column] = temp;
                break;
            case Direction.Down:
                if (row >= ColumnSize - 1) break;
                temp = newBoard[row + 1][column];
                newBoard[row + 1][column] = _board[row][column];
                newBoard[row][column] = temp;
                break;
            case Direction.Right:
                if (column >= RowSize - 1) break;
                temp = newBoard[row][column + 1];
                newBoard[row][column + 1] = _board[row][column];
                newBoard[row][column] = temp;
                break;
            case Direction.Left:
                if (column <= 0) break;
                temp = newBoard[row][column - 1];
                newBoard[row][column - 1] = _board[row][column];
                newBoard[row][column] = temp;
                break;
            default:
                throw new ArgumentException("Direction must be one of enum Board.Direction", nameof(dir));
        }

        return new Board(newBoard, false);
    }

    public void Print()
    {
        if (_board is null) throw new ArgumentException("Board can not be null");

        int xLength = _board.Length;

        if (xLength <= 0) throw new ArgumentException("Board can not be empty");

        int yLength = _board[0].Length;

        for (int i = 0; i < xLength; i++)
        {
            if (_board[i].Length != yLength) throw new ArgumentException("Inner arrays should have the same length");
        }

        string[][] values = new string[xLength][];

        int maxValLen = 0;
        for (int i = 0; i < xLength; i++)
        {
            values[i] = new string[yLength];

            for (int j = 0; j < yLength; j++)
            {
                int value = _board[i][j];
                string valueToInsert = value > 0 ? value.ToString() : String.Empty;
                int valueToInsertLen = valueToInsert.Length;

                values[i][j] = valueToInsert;
                if (valueToInsertLen > maxValLen) maxValLen = valueToInsertLen;
            }
        }

        var sb = new StringBuilder((xLength + 2) * (yLength + 2));

        int maxIndicesLen = (int)Math.Log10(xLength) + 3;
        sb.Append("".PadLeft(maxIndicesLen, ' '));
        for (int i = 0; i < yLength; i++)
        {
            string valueToInsert = (i + 1).ToString().PadLeft(maxValLen + 2);
            sb.Append(valueToInsert);
        }
        sb.Append('\n');
        for (int i = 0; i < xLength; i++)
        {
            sb.Append((i + 1).ToString().PadRight(maxIndicesLen)).Append('[');
            for (int j = 0; j < yLength; j++)
            {
                string valueToInsert = values[i][j].PadLeft(maxValLen, ' ');
                sb.Append(' ').Append(valueToInsert).Append(' ');
            }
            sb.Append("]\n");
        }

        Console.WriteLine(sb.ToString());
    }

    public override bool Equals(object? obj)
    {
        if (obj is Board board)
        {

            bool arePropertiesEqual = ColumnSize == board.ColumnSize && 
                RowSize == board.RowSize;

            if (!arePropertiesEqual) return false;

            bool areBoardFieldsEqaul = true;
            for (int i = 0; i < ColumnSize; i++)
            {
                for (int j = 0; j < RowSize; j++)
                {
                    areBoardFieldsEqaul &= this.At(i, j) == board.At(i, j);
                    if (!areBoardFieldsEqaul) return false;
                }
            }

            return true;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ColumnSize, RowSize, _board);
    }

    public static bool operator ==(Board left, Board right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Board left, Board right)
    {
        return !(left == right);
    }

    public enum Direction
    { 
        Up = 0,
        Down = 1,
        Right = 2,
        Left = 3,
    }
}
