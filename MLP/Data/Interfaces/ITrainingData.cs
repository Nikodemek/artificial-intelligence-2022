namespace MLP.Data.Interfaces;

public interface ITrainingData<U, K>
{
    public U[,] Data { get; }
    public K[] Results { get; }
    public int UniqueResults { get; }
}
