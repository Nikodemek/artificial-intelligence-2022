namespace FifteenPuzzle.PuzzleBoard;

public interface IPuzzleSolver
{
    Board Solve(in Board board);
}