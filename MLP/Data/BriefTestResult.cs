using MLP.Data.Interfaces;

namespace MLP.Data;

public record class BriefTestResult<T>(
    double GuessingAccuracy,                        // ---to moje--- procent trfaionych
    Dictionary<T, double> GuessingClassAccuracy     // ---a to nie moje ale z zadania---
    ) : ITestResult<T> where T : notnull;

