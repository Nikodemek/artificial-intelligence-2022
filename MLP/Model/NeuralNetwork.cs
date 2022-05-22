using System.Diagnostics;
using System.Xml;
using MLP.Data;
using MLP.Data.Interfaces;

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
        for (var i = 0; i < Layers[0].Neurons.Length; i++)
        {
            Layers[0].Neurons[i] = new Neuron(Array.Empty<double>());
        }

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

    public double[] FeedForward(double[] inputs)
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
        
        var lastLayer =  Layers[^1].Neurons;
        double[] output = new double[lastLayer.Length];
        for (var i = 0; i < lastLayer.Length; i++)
        {
            output[i] = lastLayer[i].Value;
        }

        return output;
    }

    public void BackPropagateErrors(double[] desiredOutput)
    {
        var lastLayer = Layers[^1].Neurons;
        List<double> errors = new List<double>();
        
        double[] actualOutput = new double[lastLayer.Length];
        for (var i = 0; i < lastLayer.Length; i++)
        {
            actualOutput[i] = lastLayer[i].Value;
        }
        
        for (var i = Layers.Length - 1; i >= 0; i--)
        {
            if (i != Layers.Length - 1)
            {
                var prevLayer = Layers[i + 1].Neurons;
                for (var j = 0; j < Layers[i].Neurons.Length; j++)
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
                for (var j = 0; j < Layers[i].Neurons.Length; j++)
                {
                    errors.Add(actualOutput[j] - desiredOutput[j]);
                }
            }

            for (var j = 0; j < Layers[i].Neurons.Length; j++)
            {
                Layers[i].Neurons[j].Delta = errors[j] * _activationFunction(Layers[i].Neurons[j].Value, true);
            }
            errors.Clear();
        }
    }

    public void UpdateWeights(double learningRate)
    {
        for (var i = 1; i < Layers.Length; i++)
        {
            var currLayer = Layers[i].Neurons;
            var prevLayer = Layers[i - 1].Neurons;
            for (var j = 0; j < currLayer.Length; j++)
            {
                for (var k = 0; k < currLayer[j].InputWeights.Length; k++)
                {
                    currLayer[j].InputWeights[k] -= learningRate * currLayer[j].Delta * prevLayer[k].Value;
                }
            }
        }
    }

    public void Train(ITrainingData<Iris> data, double learningRate, int epochCount)
    {
        for (var i = 0; i < epochCount; i++)
        {
            double error = 0;
            data.Shuffle();
            for (var j = 0; j < data.Length; j++)
            {
                double[] expected = data.RetrieveResultVector((int)data.Results[j]);

                double[] output = FeedForward(data.Data[j]);
                BackPropagateErrors(expected);
                UpdateWeights(learningRate);
                for (var k = 0; k < expected.Length; k++)
                {
                    error += (expected[k] - output[k]) * (expected[k] - output[k]);
                }
            }
            Console.WriteLine($"Epoch = {i}: learning rate: {learningRate}, error = {error}");
        }
    }
}

// Help materials:
// --> https://home.agh.edu.pl/~vlsi/AI/backp_t_en/backprop.html
// Implementation follows -> https://machinelearningmastery.com/implement-backpropagation-algorithm-scratch-python/
