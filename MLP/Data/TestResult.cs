namespace MLP.Data;

public record class TestResult<T>(
    double[][] InputData,               // wzorca wejściowego
    T[] ExpectedResults,                // pożądanego wzorca odpowiedzi
    T[] ActualResults,                  // wartości wyjściowych neuronów wyjściowych
    double GuessingAccuracy,            // ---to moje--- procent trfaionych
    double EntireError,                 // popełnionego przez sieć błędu dla całego wzorca
    double[] IndividualErrors,          // błędów popełnionych na poszczególnych wyjściach sieci
    double[][] OutputNeuronsWeights,    // wag neuronów wyjściowych
    double[][] HiddenNeuronsValues,     // wartości wyjściowych neuronów ukrytych
    double[][][] HiddenNeuronsWeights   // wag neuronów ukrytych (w kolejności warstw od dalszych względem wejść sieci do bliższych)
    );

// Help materials:
// --> https://home.agh.edu.pl/~vlsi/AI/backp_t_en/backprop.html
// Implementation follows -> https://machinelearningmastery.com/implement-backpropagation-algorithm-scratch-python/
