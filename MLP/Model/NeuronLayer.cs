namespace MLP.Model;

public class NeuronLayer
{
    public List<Neuron> Neurons { get; init; }

    public NeuronLayer(int neuronCount)
    {
        Neurons = new List<Neuron>(neuronCount);
    }
}