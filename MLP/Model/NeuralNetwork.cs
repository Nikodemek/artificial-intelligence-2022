namespace MLP.Model;

public class NeuralNetwork
{
    public List<NeuronLayer> Layers { get; init; }

    private readonly Random _random = new();
    private readonly ActivationFunction _activationFunction;

    public NeuralNetwork(ActivationFunction? activationFunction = default, params int[] neuronsInLayer)
    {
        if (neuronsInLayer.Length < 1) throw new ArgumentException("Can not pass an empty neuron count", nameof(neuronsInLayer));

        Layers = new List<NeuronLayer>(neuronsInLayer.Length) { new NeuronLayer(neuronsInLayer[0]) };

        for (var i = 1; i < Layers.Count; i++)
        {
            Layers.Add(new NeuronLayer(neuronsInLayer[i]));

            int prevLayerCount = Layers[i - 1].Neurons.Count;
            var currNeurons = Layers[i].Neurons;
            for (var j = 0; j < currNeurons.Count; j++)
            {
                var newNeuron = new Neuron(prevLayerCount);
                for (var k = 0; k < prevLayerCount; k++)
                {
                    newNeuron.InputWeights.Add(_random.NextDouble() - 0.5);
                }
                currNeurons[j] = newNeuron;
            }
        }

        _activationFunction = activationFunction ?? Functions.SigmoidUnipolar;
    }
}