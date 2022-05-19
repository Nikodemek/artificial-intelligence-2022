namespace MLP.Model;

public class Neuron
{
    public List<double> InputWeights { get; init; }
    public double Bias { get; set; }
    public double Value { get; set; }
    
    public Neuron(int inputCount)
    {
        InputWeights = new List<double>(inputCount);
    }
}