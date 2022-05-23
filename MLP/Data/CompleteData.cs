namespace MLP.Data;

public class CompleteData<T>
{
    private static readonly Random rand = new();

    public double[][] Data { get; init; }
    public T[] Results { get; init; }
    public int Length { get; init; }
    public int DataColumns { get; init; }

    private readonly int _uniqueResults;
    private readonly double[][] _resultVectors;

    public CompleteData(double[][] data, T[] results)
    {
        if (data.Length != results.Length) throw new ArgumentException("Sizes of data and results must be equal");

        Data = data;
        Results = results;
        _uniqueResults = Results.CountUnique();
        Length = results.Length;
        DataColumns = Data[0].Length;

        _resultVectors = CreateResultVectors(_uniqueResults);
    }

    public (TrainingData<T> trainingData, TestingData<T> testingData) CreateTrainingAndTestingData(double trainingToTestingRation = 0.9)
    {
        double ratio = trainingToTestingRation;
        if (ratio < 0.0 || ratio > 1.0) throw new ArgumentException("Ration must be between 0 and 1!", nameof(trainingToTestingRation));

        int trainingCount = (int)(Length * ratio);
        int testingCount = Length - trainingCount;

        var trainingData = new TrainingData<T>(this, trainingCount);
        var testingData = new TestingData<T>(this, testingCount);

        return (trainingData, testingData);
    }

    public double[] GetResultVector(int resultIndex)
    {
        return _resultVectors[resultIndex];
    }

    public void Shuffle(int minLine = -1, int maxLine = -1)
    {
        if (minLine == -1) minLine = 0;
        if (maxLine == -1) maxLine = Length;

        for (int i = minLine; i < maxLine; i++)
        {
            int index = rand.Next(minLine, maxLine);
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

public class TrainingData<T>
{
    public ArraySegment<double[]> Data { get; init; }
    public ArraySegment<T> Results { get; init; }
    public int Length { get; init; }
    public int DataColumns { get; init; }

    private readonly CompleteData<T> _completeData;
    private readonly int _offset = 0;

    public TrainingData(CompleteData<T> completeData, int count, int offset = -1)
    {
        _offset = offset == -1 ? 0 : offset;
        Length = count;
        DataColumns = completeData.DataColumns;
        Data = new ArraySegment<double[]>(completeData.Data, _offset, Length);
        Results = new ArraySegment<T>(completeData.Results, _offset, Length);

        _completeData = completeData;
    }

    public double[] GetResultVector(int resultIndex) => _completeData.GetResultVector(resultIndex);

    public void Shuffle() => _completeData.Shuffle(_offset, Length);
}

public class TestingData<T>
{
    public ArraySegment<double[]> Data { get; init; }
    public ArraySegment<T> Results { get; init; }
    public int Length { get; init; }
    public int DataColumns { get; init; }

    private readonly CompleteData<T> _completeData;
    private readonly int _offset = 0;

    public TestingData(CompleteData<T> completeData, int count, int offset = -1)
    {
        _offset = offset == -1 ? completeData.Length - count : offset;
        Length = count;
        DataColumns = completeData.DataColumns;
        Data = new ArraySegment<double[]>(completeData.Data, _offset, Length);
        Results = new ArraySegment<T>(completeData.Results, _offset, Length);

        _completeData = completeData;
    }

    public double[] GetResultVector(int resultIndex) => _completeData.GetResultVector(resultIndex);

    public void Shuffle() => _completeData.Shuffle(_offset, Length);
}