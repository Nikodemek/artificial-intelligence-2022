namespace FifteenPuzzle.PuzzleBoard;

public record RunInfo(
    bool Solved,
    string Path,
    int PathLength,
    int VisitedStates,
    int ProcessedStates,
    int MaxDepth,
    double ExecutionTime
    );