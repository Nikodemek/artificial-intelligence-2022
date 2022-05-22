using System.Diagnostics;
using MLP.Data.Interfaces;

namespace MLP.Data;

public class IrisesTrainingData : ITrainingData<Iris>
{
    public double[][] Data { get; private set; }
    public Iris[] Results { get; private set; }
    public int UniqueResults { get; private set; }
    public int Length { get; private set; }
    public int DataColumns { get; private set; }

    private static readonly Random rand = new();

    public IrisesTrainingData(double[][] data, Iris[] results)
    {
        if (data.Length != results.Length) throw new ArgumentException("Sizes of data and results must be equal");

        Data = data;
        Results = results;
        UniqueResults = Results.CountUnique();
        Length = results.Length;
        DataColumns = Data[0].Length;
    }
    
    public double[] RetrieveResultVector(int resultIndex)
    {
        Iris irisType = (Iris)resultIndex;
        
        double[] result = new double[3];
        for (var i = 0; i < 3; i++)
        {
            if ((int) irisType == i)
            {
                result[i] = 1;
            }
        }

        return result;
    }

    public void Shuffle()
    {
        int count = Length;
        for (int i = 0; i < count; i++)
        {
            int index = rand.Next(count);
            (Data[i], Data[index]) = (Data[index], Data[i]);
            (Results[i], Results[index]) = (Results[index], Results[i]);
        }
    }
}