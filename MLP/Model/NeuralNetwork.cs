namespace MLP.Model;

public class NeuralNetwork
{
    public NeuronLayer[] Layers { get; init; }

    private readonly Random _random = new();
    private readonly ActivationFunction _activationFunction;

    public NeuralNetwork(ActivationFunction? activationFunction = default, params int[] neuronsInLayer)
    {
        if (neuronsInLayer.Length < 1) throw new ArgumentException("Can not pass an empty neuron count", nameof(neuronsInLayer));

        Layers = new NeuronLayer[neuronsInLayer.Length];
        Layers[0] = new NeuronLayer(new Neuron[neuronsInLayer[0]]);

        for (var i = 1; i < Layers.Length; i++)
        {
            Layers[i] = new NeuronLayer(new Neuron[neuronsInLayer[i]]);

            int prevNeuronsCount = Layers[i - 1].Neurons.Length;
            var neurons = Layers[i].Neurons;
            for (var j = 0; j < neurons.Length; j++)
            {
                var neuron = new Neuron(new double[prevNeuronsCount])
                {
                    Bias = _random.NextDouble() - 0.5,
                    Value = 0.0,
                };
                for (var k = 0; k < prevNeuronsCount; k++)
                {
                    neuron.InputWeights[k] = _random.NextDouble() - 0.5;
                }
                neurons[j] = neuron;
            }
        }

        _activationFunction = activationFunction ?? Functions.SigmoidUnipolar;
    }

    public NeuralNetwork(NeuronLayer[] layers, ActivationFunction? activationFunction = default)
    {
        Layers = layers;

        _activationFunction = activationFunction ?? Functions.SigmoidUnipolar; ;
    }

    public NeuronLayer FeedForward(double[] inputs)
    {
        for (var i = 0; i < inputs.Length; i++)
        {
            Layers[0].Neurons[i].Value = inputs[i];
        }

        for (var i = 1; i < Layers.Length; i++)
        {
            var prevLayer = Layers[i - 1].Neurons;
            var currLayer = Layers[i].Neurons;
            for (var j = 0; j < currLayer.Length; j++)
            {
                double value = 0d;
                for (var k = 0; k < prevLayer.Length; k++)
                {
                    value += currLayer[j].InputWeights[k] * prevLayer[k].Value;
                }

                currLayer[j].Value = _activationFunction(value + currLayer[j].Bias);
            }
        }

        return Layers[^1];
    }
}