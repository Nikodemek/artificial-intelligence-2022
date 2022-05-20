namespace MLP.Data.Interfaces;

public interface ITrainingData<U, K>
{
    public U[][] Data { get; }
    public K[] Results { get; }
    public int UniqueResults { get; }
    public int Length { get; }
    public int DataColumns { get; }
}
