namespace BossPuzzle.PuzzleBoard;
using Dir = Board.Direction;

public interface IPuzzleSolver
{
    void Solve(Board board);
}