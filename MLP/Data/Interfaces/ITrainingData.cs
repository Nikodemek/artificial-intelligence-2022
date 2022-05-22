namespace MLP.Data.Interfaces;

public interface ITrainingData<K>
{
    public double[][] Data { get; }
    public K[] Results { get; }
    public int UniqueResults { get; }
    public int Length { get; }
    public int DataColumns { get; }

    public void Shuffle();
    public double[] RetrieveResultVector(int resultIndex);
}
