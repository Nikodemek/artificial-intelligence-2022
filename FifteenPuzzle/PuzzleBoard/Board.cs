using FifteenPuzzle.Utils;
using System.Text;

namespace FifteenPuzzle.PuzzleBoard;

public class Board : ICloneable, IEquatable<Board>
{
    public short ColumnSize { get; init; }
    public short RowSize { get; init; }

    public uint DistanceHammings
    {
        get
        {
            if (_distanceHammings == 0) _distanceHammings = HammingDistance(_board);
            return _distanceHammings;
        }
    }
    public uint DistanceManhattan
    {
        get
        {
            if (_distanceManhattan == 0) _distanceManhattan = ManhattanDistance(_board);
            return _distanceManhattan;
        }
    }
    public ulong Hash
    {
        get
        {
            if (_hash == 0) _hash = ComputeSmartHash(_board);
            return _hash;
        }
    }

    public int PathLength
    {
        get => _pathLength;
    }

    private readonly short[,] _board;
    private readonly short _emptyCellRow = -1;
    private readonly short _emptyCellColumn = -1;
    private readonly Board? _parent;
    private readonly ulong? _correctHash;

    private ulong _hash = 0;
    private uint _distanceHammings = 0;
    private uint _distanceManhattan = 0;
    private int _pathLength = -1;

    public Board(short[,] board)
        : this(board, true, null, null)
    { }

    private Board(short[,] board, bool copyArr, Board? parent, Direction? lastMove)
    {
        int columnLength = board.GetLength(0);

        if (columnLength <= 0) throw new ArgumentException("Board cannot be empty!");

        int rowLength = board.GetLength(1);

        if (rowLength <= 0) throw new ArgumentException("Rows cannot be empty!");

        if (copyArr)
        {
            _board = new short[columnLength, rowLength];

            for (short i = 0; i < columnLength; i++)
            {
                for (short j = 0; j < rowLength; j++)
                {
                    short value = board[i, j];
                    _board[i, j] = value;

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

            if (lastMove is not null & parent is not null)
            {
                switch (lastMove)
                {
                    case Direction.Up:
                        _emptyCellRow = (short)(parent!._emptyCellRow - 1);
                        _emptyCellColumn = parent._emptyCellColumn;
                        break;
                    case Direction.Down:
                        _emptyCellRow = (short)(parent!._emptyCellRow + 1);
                        _emptyCellColumn = parent._emptyCellColumn;
                        break;
                    case Direction.Left:
                        _emptyCellRow = parent!._emptyCellRow;
                        _emptyCellColumn = (short)(parent._emptyCellColumn - 1);
                        break;
                    case Direction.Right:
                        _emptyCellRow = parent!._emptyCellRow;
                        _emptyCellColumn = (short)(parent._emptyCellColumn + 1);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(lastMove), lastMove, "Last move out of definition.");
                }
            }
        }
        
        ColumnSize = (short)_board.GetLength(0);
        RowSize = (short)_board.GetLength(1);

        _parent = parent;
        _pathLength = parent is not null ? parent.PathLength + 1 : 0;
        _correctHash = parent is not null ? parent._correctHash : ComputeCorrectHash(rowLength, columnLength);
    }

    private static ulong ComputeCorrectHash(int rowSize, int columnSize)
    {
        var correctArray = new short[rowSize, columnSize];

        int size = correctArray.Length;
        for (var i = 0; i < rowSize; i++)
        {
            int offset = i * rowSize;
            for (var j = 0; j < columnSize; j++)
            {
                correctArray[i, j] = (short)((offset + j + 1) % size);
            }
        }

        return ComputeSmartHash(correctArray);
    }

    private static ulong ComputeSmartHash(short[,] board)
    {
        const ulong Prime = 31ul;

        ulong hash = 0;

        int rowSize = board.GetLength(0);
        int columnSize = board.GetLength(1);
        int boardSize = board.Length;
        for (int i = 0; i < rowSize; i++)
        {
            for (int j = 0; j < columnSize; j++)
            {
                hash += MathI.Power(Prime, --boardSize) * (ulong)board[i, j];
            }
        }

        return hash;
    }

    private static uint HammingDistance(short[,] board)
    {
        uint dist = 0;

        int rowSize = board.GetLength(0);
        int columnSize = board.GetLength(1);
        var boardSize = board.Length;
        for (var i = 0; i < rowSize; i++)
        {
            var offset = i * rowSize;
            for (var j = 0; j < columnSize; j++)
            {
                int value = board[i, j];
                int target = (offset + j + 1) % boardSize;

                if (value != target) dist++;
            }
        }

        return dist;
    }

    // Cos takiego probuje osiagnac tym kodem (0 to jest poprawne miejsce dla liczby).
    //      4 3 2 3 4      6 5 4 3 4      5 4 5 6 7      0 1 2 3 4
    //      3 2 1 2 3      5 4 3 2 3      4 3 4 5 6      1 2 3 4 5
    //      2 1 0 1 2      4 3 2 1 2      3 2 3 4 5      2 3 4 5 6
    //      3 2 1 2 3      3 2 1 0 1      2 1 2 3 4      3 4 5 6 7
    //      4 3 2 3 4      4 3 2 1 2      1 0 1 2 3      4 5 6 7 8
    private static uint ManhattanDistance(short[,] board)
    {
        uint dist = 0;

        int rowSize = board.GetLength(0);
        int columnSize = board.GetLength(1);
        int boardSize = board.Length;
        for (var i = 0; i < rowSize; i++)
        {
            int offset = i * rowSize;
            for (var j = 0; j < columnSize; j++)
            {
                int value = board[i, j];
                int target = (offset + j + 1) % boardSize;

                if (value == target) continue;

                int normalised = (value <= 0 ? (value + boardSize) : value) - 1;
                var (targetRow, targetColumn) = MathI.DivRem(normalised, rowSize);
                int deviation = Math.Abs(i - targetRow) + Math.Abs(j - targetColumn);

                dist += (uint)deviation;
            }
        }

        return dist;
    }

    private static Direction GetLastMove(Board child, Board parent)
    {
        int childEmptyRow = child._emptyCellRow;
        int childEmptyColumn = child._emptyCellColumn;
        int parentEmptyRow = parent._emptyCellRow;
        int parentEmptyColumn = parent._emptyCellColumn;

        if (childEmptyRow == parentEmptyRow)
        {
            if (childEmptyColumn < parentEmptyColumn) return Direction.Left;
            if (childEmptyColumn > parentEmptyColumn) return Direction.Right;
        }
        else if (childEmptyColumn == parentEmptyColumn)
        {
            if (childEmptyRow < parentEmptyRow) return Direction.Up;
            if (childEmptyRow > parentEmptyRow) return Direction.Down;
        }

        return Direction.Left;
    }

    public Board Solve(IPuzzleSolver solver)
    {
        return solver.Solve(this);
    }

    public Board Solve(IPuzzleSolverDiagnostics solver, out RunInfo runInfo)
    {
        return solver.Solve(this, out runInfo);
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
                if (_board[i, j] != target) return false;
            }
        }
        return true;
    }

    public Direction[] GetPath()
    {
        var path = new Stack<Direction>();
        for (var board = this; board._parent is not null; board = board._parent)
        {
            Direction lastMove = GetLastMove(board, board._parent);
            path.Push(lastMove);
        }
        return path.ToArray();
    }

    public string GetPathFormatted()
    {
        var sb = new StringBuilder();
        foreach (var direction in GetPath()!) sb.Append(direction.ToString()[0]);
        return sb.ToString();
    }

    public int At(int row, int column) => _board[row, column];

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

        bool hasLastDirection = false;
        Direction cancellingDir = Direction.Right;
        if (_parent is not null)
        {
            hasLastDirection = true;
            cancellingDir = GetCancellingDirection(GetLastMove(this, _parent));
        }

        foreach (var direction in directions)
        {
            if (hasLastDirection && direction == cancellingDir) continue;

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

    private Board Move(int row, int column, Direction dir)
    {
        short[,] newBoard = Arrayer.Copy(_board);

        ref short changedCell = ref newBoard[0, 0];
        switch (dir)
        {
            case Direction.Up:
                if (row <= 0) goto default;
                changedCell = ref newBoard[row - 1, column];
                break;
            case Direction.Down:
                if (row >= ColumnSize - 1) goto default;
                changedCell = ref newBoard[row + 1, column];
                break;
            case Direction.Right:
                if (column >= RowSize - 1) goto default;
                changedCell = ref newBoard[row, column + 1];
                break;
            case Direction.Left:
                if (column <= 0) goto default;
                changedCell = ref newBoard[row, column - 1];
                break;
            default:
                return this;
        }

        short temp = changedCell;
        changedCell = _board[row, column];
        newBoard[row, column] = temp;

        return new Board(newBoard, false, this, dir);
    }

    private static Direction GetCancellingDirection(Direction direction)
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

    public object Clone()
    {
        var newBoardTable = new short[RowSize, ColumnSize];
        for (var i = 0; i < RowSize; i++)
        {
            for (var j = 0; j < ColumnSize; j++)
            {
                newBoardTable[i, j] = _board[i, j];
            }
        }

        return new Board(newBoardTable);
    }

    public void Print()
    {
        if (_board is null) throw new ArgumentException("Board can not be null");

        int xLength = _board.GetLength(0);

        if (xLength <= 0) throw new ArgumentException("Board can not be empty");

        int yLength = _board.GetLength(1);

        string[][] values = new string[xLength][];

        int maxValLen = 0;
        for (int i = 0; i < xLength; i++)
        {
            values[i] = new string[yLength];

            for (int j = 0; j < yLength; j++)
            {
                int value = _board[i, j];
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

    public bool Equals(Board? other)
    {
        if (other is null) return false;

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
                areBoardFieldsEqual &= this._board[i, j] == other._board[i, j];
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
        public bool Equals(Board? x, Board? y)
        {
            if (x is null || y is null) return false;
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
