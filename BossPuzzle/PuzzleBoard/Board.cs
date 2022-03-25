using System;
using System.Text;
using BossPuzzle.Utils;

namespace BossPuzzle.PuzzleBoard;

public readonly struct Board : ICloneable, IEquatable<Board>
{
    public static uint instances = 0;

    public short ColumnSize { get; init; }
    public short RowSize { get; init; }
    public ulong Hash { get; init; }
    public uint Hammings { get; init; }
    public uint Manhattans { get; init; }

    private readonly short[][] _board;
    private readonly short _emptyCellRow = -1;
    private readonly short _emptyCellColumn = -1;
    private readonly List<Direction> _path;
    private readonly ulong? _correctHash;

    public Board(short[][] board)
        : this(board, true, null, null)
    { }

    private Board(short[][] board, bool copyArr, List<Direction>? path, ulong? correctHash)
        : this()
    {
        int columnLength = board.Length;

        if (columnLength <= 0) throw new ArgumentException("Board cannot be empty!");

        int rowLength = board[0].Length;

        if (rowLength <= 0) throw new ArgumentException("Rows cannot be empty!");

        if (copyArr)
        {
            _board = new short[columnLength][];

            for (short i = 0; i < columnLength; i++)
            {
                int newRowLength = board[i].Length;

                if (newRowLength != rowLength) throw new ArgumentException("Rows sizes must be equal");

                rowLength = newRowLength;
                _board[i] = new short[rowLength];

                for (short j = 0; j < rowLength; j++)
                {
                    short value = board[i][j];
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
            _board = board;

            for (short i = 0; i < columnLength; i++)
            {
                for (short j = 0; j < rowLength; j++)
                {
                    if (board[i][j] <= 0)
                    {
                        _emptyCellRow = i;
                        _emptyCellColumn = j;
                    }
                }
            }
        }

        RowSize = (short)rowLength;
        ColumnSize = (short)columnLength;
        Hash = ComputeSmartHash(board, rowLength, columnLength);
        Hammings = HammingDistance(board, rowLength, columnLength);
        Manhattans = ManhattanDistance(board, rowLength, columnLength);

        _path = path ?? new List<Direction>();
        _correctHash = correctHash ?? ComputeCorrectHash(rowLength, columnLength);

        instances++;
    }

    private static ulong ComputeCorrectHash(int rowSize, int columnSize)
    {
        var correctArray = new short[rowSize][];
        
        int size = rowSize * columnSize;
        for (var i = 0; i < rowSize; i++)
        {
            int offset = i * rowSize;
            correctArray[i] = new short[columnSize];

            for (var j = 0; j < columnSize; j++)
            {
                correctArray[i][j] = (short)((offset + j + 1) % size);
            }
        }

        return ComputeSmartHash(correctArray, rowSize, columnSize);
    }

    private static ulong ComputeSmartHash(short[][] board, int rowSize, int columnSize)
    {
        const ulong Prime = 31ul;

        int boardSize = rowSize * columnSize - 1;
        ulong hash = 0;

        for (int i = 0; i < rowSize; i++)
        {
            for (int j = 0; j < columnSize; j++)
            {
                hash += MathI.Power(Prime, boardSize--) * (ulong)board[i][j];
            }
        }

        return hash;
    }

    private static uint HammingDistance(short[][] board, int rowSize, int columnSize)
    {
        uint dist = 0;

        var size = rowSize * columnSize;
        for (var i = 0; i < rowSize; i++)
        {
            var offset = i * rowSize;
            for (var j = 0; j < columnSize; j++)
            {
                int value = board[i][j];
                int target = (offset + j + 1) % size;

                if (value != target) dist++;
            }
        }

        return dist;
    }

    // Cos takiego probuje osiagnac tym kodem (0 to jest poprawne miejsce dla liczby).
    //     4 3 2 3 4     6 5 4 3 4     5 4 5 6 7     0 1 2 3 4
    //     3 2 1 2 3     5 4 3 2 3     4 3 4 5 6     1 2 3 4 5
    //     2 1 0 1 2     4 3 2 1 2     3 2 3 4 5     2 3 4 5 6
    //     3 2 1 2 3     3 2 1 0 1     2 1 2 3 4     3 4 5 6 7
    //     4 3 2 3 4     4 3 2 1 2     1 0 1 2 3     4 5 6 7 8
    private static uint ManhattanDistance(short[][] board, int rowSize, int columnSize)
    {
        const uint Strength = 2;

        uint dist = 0;

        int size = rowSize * columnSize;
        for (var i = 0; i < rowSize; i++)
        {
            int offset = i * rowSize;
            for (var j = 0; j < columnSize; j++)
            {
                int value = board[i][j];
                int target = (offset + j + 1) % size;

                if (value == target) continue;

                int normalised = (value <= 0 ? (value + size) : value) - 1;
                (int targetRow, int targetColumn) = MathI.DivRem(normalised, rowSize);
                int deviation = Math.Abs(i - targetRow) + Math.Abs(j - targetColumn);

                dist += MathI.Power(Strength, deviation);
            }
        }

        return dist;
    }

    public Board Solve(IPuzzleSolver solver)
    {
        return solver.Solve(this);
    }
    
    public bool IsValid()
    {
        if (Hash != _correctHash) return false;

        int size = RowSize * ColumnSize;
        for (var i = 0; i < RowSize; i++)
        {
            int offset = i * RowSize;
            for (var j = 0; j < ColumnSize; j++)
            {
                int target = (offset + j + 1) % size;
                if (_board[i][j] != target) return false;
            }
        }
        return true;
    }

    public Direction[] GetPath() => _path.ToArray();

    public int At(int row, int column) => _board[row][column];

    public Direction[] ClarifyMovement()
    {
        return ClarifyMovement(
            new[] {
                Direction.Up, 
                Direction.Down, 
                Direction.Left, 
                Direction.Right,
            });
    }

    public Direction[] ClarifyMovement(Direction[] directions)
    {
        var newDirections = new List<Direction>(3);
        foreach (var direction in directions)
        {
            if (_path?.Count > 0 && direction == GetCancellingDirection(_path[^1])) continue;

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
                    throw new ArgumentException($"Not recognized Direction ({direction})");
            }  

            newDirections.Add(direction);
        }

        return newDirections.ToArray();
    }

    public Board Move(Direction dir)
    {
        return Move(_emptyCellRow, _emptyCellColumn, dir);
    }

    public Board Move(int row, int column, Direction dir)
    {
        short[][] newBoard = Arrayer.Copy(_board);

        ref short changedCell = ref newBoard[0][0];
        switch (dir)
        {
            case Direction.Up:
                if (row <= 0) goto default;
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
                return this;
        }

        short temp = changedCell;
        changedCell = _board[row][column];
        newBoard[row][column] = temp;

        var newPath = new List<Direction>(_path) { dir };

        return new Board(newBoard, false, newPath, _correctHash);
    }

    public object Clone()
    {
        var newBoardTable = new short[RowSize][];
        for (var i = 0; i < RowSize; i++)
        {
            newBoardTable[i] = new short[ColumnSize];
            for (var j = 0; j < ColumnSize; j++)
            {
                newBoardTable[i][j] = _board[i][j];
            }
        }

        return new Board(newBoardTable);
    }

    public static Direction GetCancellingDirection(Direction direction)
    {
        return direction switch
        {
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            Direction.Right => Direction.Left,
            Direction.Left => Direction.Right,
            _ => throw new ArgumentException("Not recognized Direction.", nameof(direction)),
        };
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
        bool arePropertiesEqual = (
            ColumnSize == other.ColumnSize &&
            RowSize == other.RowSize
            );

        if (!arePropertiesEqual) return false;

        bool areBoardFieldsEqual = true;
        for (int i = 0; i < ColumnSize; i++)
        {
            for (int j = 0; j < RowSize; j++)
            {
                areBoardFieldsEqual &= this.At(i, j) == other.At(i, j);
                if (!areBoardFieldsEqual) return false;
            }
        }

        return true;
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
