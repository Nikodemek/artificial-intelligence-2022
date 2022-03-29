namespace BossPuzzle.PuzzleBoard;

public interface IPuzzleSolverDiagnostics
{
    Board Solve(in Board board, out RunInfo runInfo);
}