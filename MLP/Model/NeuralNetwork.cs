namespace MLP.Model;

public class NeuralNetwork
{
    public List<NeuronLayer> Layers { get; set; }

    private Random _random = new Random();

    public NeuralNetwork(params int[] neuronsInLayer)
    {
        Layers = new List<NeuronLayer>(neuronsInLayer.Length);
        foreach (var count in neuronsInLayer)
        {
            Layers.Add(new NeuronLayer(count));
        }

        for (var i = 1; i < Layers.Count; i++)
        {
            int prevLayerCount = Layers[i - 1].Neurons.Count;
            for (var j = 0; j < Layers[i].Neurons.Count; j++)
            {
                for (var k = 0; k < prevLayerCount; k++)
                {
                    Neuron newNeuron = new Neuron(prevLayerCount);
                    newNeuron.InputWeights.Add(_random.NextDouble() % 1.0d - 0.5d);
                    Layers[i].Neurons[k] = newNeuron;
                }
            }
        }
    }
}