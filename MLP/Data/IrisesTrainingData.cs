using MLP.Data.Interfaces;

namespace MLP.Data;

public class IrisesTrainingData : ITrainingData<double, IrisType>
{
    public double[,] Data { get; private set; }
    public IrisType[] Results { get; private set; }
    public int UniqueResults { get; private set; }

    public IrisesTrainingData(double[,] data, IrisType[] results)
    {
        Data = data;
        Results = results;
        UniqueResults = Results.CountUnique();
    }
}