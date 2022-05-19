namespace MLP.Model;

public class NeuronLayer
{
    public List<Neuron> Neurons { get; set; }

    public NeuronLayer(int neuronCount)
    {
        Neurons = new List<Neuron>(neuronCount);
    }
}