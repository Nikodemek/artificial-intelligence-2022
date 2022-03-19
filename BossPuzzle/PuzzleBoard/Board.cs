using System;
using System.Text;
using BossPuzzle.Utils;

namespace BossPuzzle.PuzzleBoard;

public struct Board : ICloneable, IEquatable<Board>
{
    public int ColumnSize { get; init; }
    public int RowSize { get; init; }
    public ulong Hash
    {
        get
        {
            if (_hash <= 0) _hash = this.ComputeSmartHash();
            return _hash;
        }
    }

    private readonly int[][] _board;
    private readonly int _emptyCellRow = -1;
    private readonly int _emptyCellColumn = -1;

    public List<Direction> SolvePath { get; set; }

    private ulong _hash = 0;

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
                        _emptyCellRow = i;
                        _emptyCellColumn = j;
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
                        _emptyCellRow = i;
                        _emptyCellColumn = j;
                    }
                }
            }
        }

        RowSize = rowLength;
        ColumnSize = columnLength;

        SolvePath = new List<Direction>();
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

    public Direction[] ClarifyMovement(Direction[] directions)
    {
        var newDirections = new List<Direction>(4);
        foreach (var direction in directions)
        {
            switch (direction)
            {
                case Direction.Up:
                    if (_emptyCellRow <= 0) continue;
                    break;
                case Direction.Down:
                    if (_emptyCellRow >= ColumnSize - 1) continue;
                    break;
                case Direction.Right:
                    if (_emptyCellColumn >= RowSize - 1) continue;
                    break;
                case Direction.Left:
                    if (_emptyCellColumn <= 0) continue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }  

            newDirections.Add(direction);
        }

        return newDirections.ToArray();
    }

    // Used to move an empty cell
    public Board Move(Direction dir)
    {
        return Move(_emptyCellRow, _emptyCellColumn, dir);
    }

    // Used to move given cell
    public Board Move(int row, int column, Direction dir)
    {
        int[][] newBoard = Cloner.DoubleArr(_board);

        //...oh, that's how
        ref int changedCell = ref newBoard[0][0];
        switch (dir)
        {
            case Direction.Up:
                if (row <= 0) goto default; // GOTO RZONDZI (na wspolbieznym nie bede tak odpierdzielal, ale ladnie to wyglada, a tak btw to w switchach czasami sie widuje goto 'ktorys_przypadek')
                changedCell = ref newBoard[row - 1][column];
                break;
            case Direction.Down:
                if (row >= ColumnSize - 1) goto default;
                changedCell = ref newBoard[row + 1][column];
                break;
            case Direction.Right:
                if (column >= RowSize - 1) goto default;
                changedCell = ref newBoard[row][column + 1];
                break;
            case Direction.Left:
                if (column <= 0) goto default;
                changedCell = ref newBoard[row][column - 1];
                break;
            default:
                return new Board(newBoard, false);
        }

        int temp = changedCell;
        changedCell = _board[row][column];
        newBoard[row][column] = temp;
        
        var changedBoard = new Board(newBoard, false);
        SolvePath.ForEach(direction => changedBoard.SolvePath.Add(direction));

        return changedBoard;
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
        if (obj is Board board) return Equals(board);
        return false;
    }

    public bool Equals(Board other)
    {
        bool arePropertiesEqual = ColumnSize == other.ColumnSize &&
                RowSize == other.RowSize;

        if (!arePropertiesEqual) return false;

        bool areBoardFieldsEqaul = true;
        for (int i = 0; i < ColumnSize; i++)
        {
            for (int j = 0; j < RowSize; j++)
            {
                areBoardFieldsEqaul &= this.At(i, j) == other.At(i, j);
                if (!areBoardFieldsEqaul) return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ColumnSize, RowSize, _board);
    }

    public object Clone()
    {
        var newBoardTable = new int[RowSize][];
        for (var i = 0; i < RowSize; i++)
        {
            newBoardTable[i] = new int[ColumnSize];
            for (var j = 0; j < ColumnSize; j++)
            {
                newBoardTable[i][j] = _board[i][j];
            }
        }

        return new Board(newBoardTable);
    }

    public static bool operator ==(Board left, Board right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Board left, Board right)
    {
        return !(left == right);
    }

    private ulong ComputeSmartHash()
    {
        const ulong Prime = 31ul;

        int boardSize = RowSize * ColumnSize - 1;
        ulong hash = 0;

        for (int i = 0; i < RowSize; i++)
        {
            for (int j = 0; j < ColumnSize; j++)
            {
                hash += MathI.Power(Prime, boardSize--) * (ulong)_board[i][j];
            }
        }

        return hash;
    }

    public enum Direction
    { 
        Up = 0,
        Down = 1,
        Right = 2,
        Left = 3,
    }

    public class Comparer : IEqualityComparer<Board>
    {
        public bool Equals(Board x, Board y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Board obj)
        {
            ulong hash = obj.Hash;
            ulong maxInt32 = (ulong)Int32.MaxValue;
            return (int)(hash % maxInt32);
        }
    }
}
