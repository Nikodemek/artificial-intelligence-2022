using System;
using System.Text;

namespace BossPuzzle.PuzzleBoard;

public struct Board
{
    public int ColumnSize { get; init; }
    public int RowSize { get; init; }

    readonly int[][] _board;

    public Board(int[][] board) : this()
    {
        int columnLength = board.Length;

        if (columnLength <= 0) throw new ArgumentException("Board cannot be empty!");

        int rowLength = board[0].Length;

        if (rowLength <= 0) throw new ArgumentException("Rows cannot be empty!");

        this._board = new int[columnLength][];

        for (var i = 0; i < columnLength; i++)
        {
            int newRowLength = board[i].Length;

            if (newRowLength != rowLength) throw new ArgumentException("Rows sizes must be equal");

            rowLength = newRowLength;
            _board[i] = new int[rowLength];

            for (var j = 0; j < rowLength; j++)
            {
                _board[i][j] = board[i][j];
            }
        }

        ColumnSize = columnLength;
        RowSize = rowLength;
    }

    public int At(int row, int column)
    {
        return _board[row][column];
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
                string valueToInsert = _board[i][j].ToString();
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
        Up,
        Down,
        Left,
        Right,
    }
}
