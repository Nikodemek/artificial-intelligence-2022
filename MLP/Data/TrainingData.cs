using System.Diagnostics;
using MLP.Data.Interfaces;

namespace MLP.Data;

public class TrainingData : TrainingData<Iris>
{
    public TrainingData(double[][] data, Iris[] results)
    {
        if (data.Length != results.Length) throw new ArgumentException("Sizes of data and results must be equal");

        Data = data;
        Results = results;
        UniqueResults = Results.CountUnique();
        Length = results.Length;
        DataColumns = Data[0].Length;
    }
    
    public override double[] RetrieveResultVector(int resultIndex)
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
}