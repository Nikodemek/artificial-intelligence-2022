namespace MLP.Model;

public class Neuron
{
    public List<double> InputWeights { get; init; }
    
    public Neuron(int inputCount)
    {
        InputWeights = new List<double>(inputCount);
    }
}