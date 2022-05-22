using System.Diagnostics;
using MLP.Data.Interfaces;

namespace MLP.Data;

public class IrisesTrainingData : ITrainingData<double, IrisType>
{
    public double[][] Data { get; private set; }
    public IrisType[] Results { get; private set; }
    public int UniqueResults { get; private set; }
    public int Length { get; private set; }
    public int DataColumns { get; private set; }

    public IrisesTrainingData(double[][] data, IrisType[] results)
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
        IrisType irisType = IrisType.Setosa;
        switch (resultIndex)
        {
            case 0:
                irisType = IrisType.Setosa;
                break;
            case 1:
                irisType = IrisType.Versicolour;
                break;
            case 2:
                irisType = IrisType.Virginica;
                break;
        }
        
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