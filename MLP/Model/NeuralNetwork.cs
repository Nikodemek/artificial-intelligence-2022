using System.Diagnostics;
using System.Globalization;
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

    public double[] FeedForward(double[] inputs, bool biasFlag = true)
    {
        for (var i = 0; i < inputs.Length; i++)
        {
            Layers[0].Neurons[i].Value = inputs[i];
        }

        for (var i = 1; i < Layers.Length; i++)
        {
            var prevNeurons = Layers[i - 1].Neurons;
            var currNeurons = Layers[i].Neurons;
            for (var j = 0; j < currNeurons.Length; j++)
            {
                var currNeuron = currNeurons[j];
                double value = 0.0;
                for (var k = 0; k < prevNeurons.Length; k++)
                {
                    value += currNeuron.InputWeights[k] * prevNeurons[k].Value;
                }
                currNeuron.PrevValue = currNeuron.Value;
                currNeuron.Value = _activationFunction(value + (biasFlag ? currNeuron.Bias : 0));
            }
        }

        var lastLayer = Layers[^1].Neurons;
        double[] output = new double[lastLayer.Length];
        for (var i = 0; i < lastLayer.Length; i++)
        {
            output[i] = lastLayer[i].Value;
        }

        return output;
    }

    private void BackPropagateErrors(double[] desiredOutput)
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
            var currLayer = Layers[i];
            if (i != Layers.Length - 1)
            {
                var prevLayer = Layers[i + 1].Neurons;
                for (var j = 0; j < currLayer.Neurons.Length; j++)
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
                for (var j = 0; j < currLayer.Neurons.Length; j++)
                {
                    errors.Add(actualOutput[j] - desiredOutput[j]);
                }
            }

            for (var j = 0; j < currLayer.Neurons.Length; j++)
            {
                var currNeuron = currLayer.Neurons[j];
                currNeuron.PrevDelta = currNeuron.Delta;
                currNeuron.Delta = errors[j] * _activationFunction(currNeuron.Value, true);
            }
            errors.Clear();
        }
    }

    private void UpdateWeights(double learningRate, double momentum = 0.9)
    {
        for (var i = 1; i < Layers.Length; i++)
        {
            var currNeurons = Layers[i].Neurons;
            var prevNeurons = Layers[i - 1].Neurons;
            for (var j = 0; j < currNeurons.Length; j++)
            {
                var currNeuron = currNeurons[j];
                currNeuron.Bias -= learningRate * currNeuron.Delta;
                for (var k = 0; k < currNeuron.InputWeights.Length; k++)
                {
                    var prevNeuron = prevNeurons[k];
                    double momentumEffect = momentum * (currNeuron.PrevDelta * prevNeuron.PrevValue);
                    currNeuron.InputWeights[k] -= learningRate * currNeuron.Delta * prevNeuron.Value + momentumEffect;
                }
            }
        }
    }

    public void Train<T>(TrainingData<T> data, double learningRate, int epochCount = 0, double errorAccuracy = 0, bool shuffleFlag = false) where T : IConvertible
    {
        if (epochCount < 0 && errorAccuracy < 0) return;

        double minError = Double.MaxValue;
        int minErrorEpoch = 0;
        int i = 0;
        int lastImprovement = 0;

        while (true)
        {
            if (epochCount > 0 && i >= epochCount) break;
            if (errorAccuracy > 0 && errorAccuracy >= minError || lastImprovement > 100) break;

            if (shuffleFlag) data.Shuffle();

            double error = 0;
            for (var j = 0; j < data.Length; j++)
            {
                int resultVectorIndex = data.Results[j].ToInt32(NumberFormatInfo.InvariantInfo);
                double[] expected = data.GetResultVector(resultVectorIndex);
                double[] output = FeedForward(data.Data[j]);

                BackPropagateErrors(expected);
                UpdateWeights(learningRate);
                for (var k = 0; k < expected.Length; k++)
                {
                    double diff = expected[k] - output[k];
                    error += diff * diff;
                }
            }

            if (error < minError)
            {
                minError = error;
                minErrorEpoch = i;
                lastImprovement = 0;
            }

            Console.WriteLine($"Epoch = {i}: learning rate: {learningRate}, error = {error:g4}");
            i++;
            lastImprovement++;
        }
        Console.WriteLine($"Min error = {minError} occured in epoch {minErrorEpoch}");
    }
}

// Help materials:
// --> https://home.agh.edu.pl/~vlsi/AI/backp_t_en/backprop.html
// Implementation follows -> https://machinelearningmastery.com/implement-backpropagation-algorithm-scratch-python/