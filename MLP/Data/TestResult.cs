using MLP.Data.Interfaces;

namespace MLP.Data;

public record class FullTestResult<T>(
    double[][] InputData,               // wzorca wejściowego
    T[] ExpectedResults,                // pożądanego wzorca odpowiedzi
    T[] ActualResults,                  // wartości wyjściowych neuronów wyjściowych
    double GuessingAccuracy,            // ---to moje--- procent trfaionych
    Dictionary<T, double> GuessingClassAccuracy,     // ---a to nie moje ale z zadania---
    double[] EntireError,                 // popełnionego przez sieć błędu dla całego wzorca
    double[][] IndividualErrors,          // błędów popełnionych na poszczególnych wyjściach sieci
    double[][] OutputNeuronsWeights,    // wag neuronów wyjściowych
    double[][] HiddenNeuronsValues,     // wartości wyjściowych neuronów ukrytych
    double[][][] HiddenNeuronsWeights   // wag neuronów ukrytych (w kolejności warstw od dalszych względem wejść sieci do bliższych)
    ) : ITestResult<T> where T : notnull;
