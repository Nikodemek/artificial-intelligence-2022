namespace BossPuzzle.PuzzleBoard;

public interface IPuzzleSolver
{
    Board Solve(in Board board);
}