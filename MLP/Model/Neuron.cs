namespace MLP.Model;

public class Neuron
{
    public List<double> InputWeights { get; set; }
    
    public Neuron(int inputCount)
    {
        InputWeights = new List<double>(inputCount);
    }
}