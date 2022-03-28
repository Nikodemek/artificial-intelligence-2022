namespace BossPuzzle.PuzzleBoard;

public interface IPuzzleSolver
{
    Board Solve(in Board board);
    uint GetBoardInstanceCount();
    uint GetMaxDepthAchieved();

    double GetTimeConsumed();
}