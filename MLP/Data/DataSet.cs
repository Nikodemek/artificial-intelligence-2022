namespace MLP.Data;

public class DataSet<T>
{
    public double[][] Data { get; init; }
    public T[] Results { get; init; }
    public int Length { get; init; }
    public int DataColumns { get; init; }
    public int Classes { get; init; }

    private readonly Random _rand = new();
    private readonly double[][] _resultVectors;

    public DataSet(double[][] data, T[] results, bool shuffled = false)
    {
        if (data.Length != results.Length) throw new ArgumentException("Sizes of data and results must be equal");

        Data = data;
        Results = results;
        Classes = Results.CountUnique();
        Length = results.Length;
        DataColumns = Data[0].Length;

        _resultVectors = CreateResultVectors(Classes);

        if (shuffled) Shuffle();
    }

    private DataSet(DataSet<T> original, int offset, int count, bool shuffled = false)
    {
        int exUpperBound = offset + count;
        Data = original.Data[offset..exUpperBound];
        Results = original.Results[offset..exUpperBound];
        Length = count;
        DataColumns = original.DataColumns;
        Classes = original.Classes;

        _resultVectors = original._resultVectors.GClone();

        if (shuffled) Shuffle();
    }

    public (DataSet<T> trainingData, DataSet<T> testingData) CreateTrainingAndTestingData(double trainingToTestingRation = 0.8, bool shuffled = false)
    {
        double ratio = trainingToTestingRation;
        if (ratio < 0.0 || ratio > 1.0) throw new ArgumentException("Ration must be between 0 and 1!", nameof(trainingToTestingRation));

        if (shuffled) Shuffle();

        int trainingCount = (int)(Length * ratio);
        int testingCount = Length - trainingCount;

        var trainingData = new DataSet<T>(this, 0, trainingCount, shuffled);
        var testingData = new DataSet<T>(this, trainingCount, testingCount, shuffled);

        return (trainingData, testingData);
    }

    public double[] GetResultVector(int resultIndex)
    {
        return _resultVectors[resultIndex];
    }

    public void Shuffle()
    {
        for (int i = 0; i < Length; i++)
        {
            int index = _rand.Next(0, Length);
            (Data[i], Data[index]) = (Data[index], Data[i]);
            (Results[i], Results[index]) = (Results[index], Results[i]);
        }
    }

    private static double[][] CreateResultVectors(int uniqueCount)
    {
        var vectors = new double[uniqueCount][];

        for (int i = 0; i < uniqueCount; i++)
        {
            var vector = new double[uniqueCount];
            vector[i] = 1.0;
            vectors[i] = vector;
        }

        return vectors;
    }
}