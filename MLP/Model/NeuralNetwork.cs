using System.Xml;
using MLP.Data;

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

                newNeuron.Bias = _random.NextDouble() - 0.5;
                currNeurons[j] = newNeuron;
            }
        }

        _activationFunction = activationFunction ?? Functions.SigmoidUnipolar;
    }

    public double[] FeedForward(double[] inputs)
    {
        for (var i = 0; i < inputs.Length; i++)
        {
            Layers[0].Neurons[i].Value = inputs[i];
        }

        for (var i = 1; i < Layers.Count; i++)
        {
            var prevLayer = Layers[i - 1].Neurons;
            var currLayer = Layers[i].Neurons;
            for (var j = 0; j < currLayer.Count; j++)
            {
                double value = 0d;
                for (var k = 0; k < prevLayer.Count; k++)
                {
                    value += currLayer[j].InputWeights[k] * prevLayer[k].Value;
                }

                currLayer[j].Value = _activationFunction(value + currLayer[j].Bias);
            }
        }
        
        var lastLayer =  Layers[^1].Neurons;
        double[] output = new double[lastLayer.Count];
        for (var i = 0; i < lastLayer.Count; i++)
        {
            output[i] = lastLayer[i].Value;
        }

        return output;
    }

    // Implementation follows -> https://machinelearningmastery.com/implement-backpropagation-algorithm-scratch-python/
    public void BackPropagateErrors(double[] desiredOutput)
    {
        var lastLayer = Layers[^1].Neurons;
        List<double> errors = new List<double>();
        
        double[] actualOutput = new double[lastLayer.Count];
        for (var i = 0; i < lastLayer.Count; i++)
        {
            actualOutput[i] = lastLayer[i].Value;
        }
        
        for (var i = Layers.Count - 1; i >= 0; i--)
        {
            if (i != Layers.Count - 1)
            {
                var prevLayer = Layers[i + 1].Neurons;
                for (var j = 0; j < Layers[i].Neurons.Count; j++)
                {
                    double error = 0d;
                    foreach (var neuron in prevLayer)
                    {
                        error += neuron.InputWeights[j] * neuron.Delta;
                    }
                    errors.Add(error);
                }
            }
            else
            {
                for (var j = 0; j < Layers[i].Neurons.Count; j++)
                {
                    errors.Add(actualOutput[j] - desiredOutput[j]);
                }
            }

            for (var j = 0; j < Layers[i].Neurons.Count; j++)
            {
                Layers[i].Neurons[j].Delta = errors[j] * _activationFunction(actualOutput[i], true);
            }
        }
    }
}