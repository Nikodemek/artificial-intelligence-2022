using MLP.Data.Interfaces;

namespace MLP.Data;

public class IrisesTrainingData : ITrainingData<double, IrisType>
{
    public double[,] Data { get; private set; }
    public IrisType[] Results { get; private set; }
    public int UniqueResults { get; private set; }
    public int Length { get; private set; }
    public int DataColumns { get; private set; }

    public IrisesTrainingData(double[,] data, IrisType[] results)
    {
        if (data.GetLength(0) != results.Length) throw new ArgumentException("Sizes of data and results must be equal");

        Data = data;
        Results = results;
        UniqueResults = Results.CountUnique();
        Length = results.Length;
        DataColumns = Data.GetLength(1);
    }
    
    public double[] RetrieveResultVector(int resultIndex)
    {
        IrisType irisType = Results[resultIndex];
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