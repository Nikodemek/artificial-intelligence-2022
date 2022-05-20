namespace MLP.Model;

public class NeuronLayer
{
    public Neuron[] Neurons { get; init; }

    public NeuronLayer(Neuron[] neurons)
    {
        Neurons = neurons;
    }
}