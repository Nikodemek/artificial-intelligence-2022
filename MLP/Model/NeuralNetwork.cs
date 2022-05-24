﻿using MLP.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MLP.Model;

public class NeuralNetwork<T> where T : IConvertible
{
    private const string ErrorDataFileName = "errors.txt";

    public NeuronLayer[] Layers { get; init; }

    private readonly Random _random = new();
    private readonly ActivationFunction _activationFunction;
    private readonly Func<int, T> _converter;
    private readonly TypeCode _typeCode;
    private readonly EqualityComparer<T> _comparer;

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
        _typeCode = Type.GetTypeCode(typeof(T));
        _converter = IntToTypeDefaultConverter;
        _comparer = EqualityComparer<T>.Default;
    }
    public NeuralNetwork(NeuronLayer[] layers, ActivationFunction? activationFunction = default, Func<int, T>? intToTypeConverter = default)
    {
        Layers = layers;

        _activationFunction = activationFunction ?? Functions.SigmoidUnipolar; ;
        _typeCode = Type.GetTypeCode(typeof(T));
        _converter = intToTypeConverter ?? IntToTypeDefaultConverter;
        _comparer = EqualityComparer<T>.Default;
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
        var errors = new List<double>();

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
                    double error = 0.0;
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

    private void UpdateWeights(double learningRate, double momentum)
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

    public void Train(DataSet<T> data, double learningRate, int epochCount = 0, double errorAccuracy = 0, double momentum = 0.0, bool shuffleFlag = false)
    {
        if (epochCount < 0 && errorAccuracy < 0) return;

        var testData = new PlainDataFileManager(ErrorDataFileName);
        var stringBuilder = new StringBuilder();

        double minError = Double.MaxValue;
        int minErrorEpoch = 0;

        for (int i = 0, lastImprovement = 0; epochCount == 0 || i < epochCount; i++, lastImprovement++)
        {
            if (errorAccuracy > 0) if (errorAccuracy >= minError || lastImprovement > 100) break;

            if (shuffleFlag) data.Shuffle();

            double error = 0.0;
            for (var j = 0; j < data.Length; j++)
            {
                int resultVectorIndex = data.Results[j].ToInt32(NumberFormatInfo.InvariantInfo);
                double[] expected = data.GetResultVector(resultVectorIndex);

                double[] output = FeedForward(data.Data[j]);
                BackPropagateErrors(expected);
                UpdateWeights(learningRate, momentum);
                for (var k = 0; k < expected.Length; k++)
                {
                    double diff = expected[k] - output[k];
                    error += diff * diff;
                }
            }

            if (i % 10 == 0) stringBuilder.AppendLine(error.ToString(CultureInfo.InvariantCulture));

            if (error < minError)
            {
                minError = error;
                minErrorEpoch = i;
                lastImprovement = 0;
            }

            Console.WriteLine($"Epoch = {i}: learning rate: {learningRate}, error = {error:g4}");
        }
        Console.WriteLine($"Min error = {minError} occured in epoch {minErrorEpoch}");

        testData.Write(stringBuilder.ToString());
    }

    public TestResult<T> Test(DataSet<T> testingData, bool biasFlag = true)
    {
        int length = testingData.Length;
        int hiddenLayersCount = Layers.Length - 2;

        int correct = 0;
        var actualResults = new T[length];
        double[][] hiddenNeuronsValues = new double[hiddenLayersCount][];
        double[][][] hiddenNeuronsWeights = new double[hiddenLayersCount][][]; // yooooooooooooooooooooooo

        for (int i = 0; i < length; i++)
        {
            double[] output = FeedForward(testingData.Data[i], biasFlag);
            int maxIndex = 0;
            for (int j = 1; j < output.Length; j++)
            {
                if (output[j] > output[maxIndex]) maxIndex = j;
            }
            T result = _converter(maxIndex);
            if (_comparer.Equals(result, testingData.Results[i])) correct++;
            actualResults[i] = result;
        }

        for (int i = 1; i < hiddenLayersCount + 1; i++)
        {
            var currNeurons = Layers[i].Neurons;
            int currNeuronsCount = currNeurons.Length;

            var neuronsValues = new double[currNeuronsCount];
            var neuronsWeights = new double[currNeuronsCount][];
            for (int j = 0; j < currNeuronsCount; j++)
            {
                var currNeuron = currNeurons[j];
                neuronsValues[j] = currNeuron.Value;
                neuronsWeights[j] = currNeuron.InputWeights;
            }
            hiddenNeuronsValues[i-1] = neuronsValues;
            hiddenNeuronsWeights[i-1] = neuronsWeights;
        }

        return new TestResult<T>(
            testingData.Data,
            testingData.Results,
            actualResults,
            (double)correct / (double)length,
            -1.0,                       // To trzeba wytrzasnąć
            Array.Empty<double>(),                // To tesz
            Layers[^1].Neurons.Select(n => n.InputWeights).ToArray(),
            hiddenNeuronsValues,
            hiddenNeuronsWeights
            );
    }

    private T IntToTypeDefaultConverter(int s)
    {
        return (T)Convert.ChangeType(s, _typeCode);
    }
}

// Help materials:
// --> https://home.agh.edu.pl/~vlsi/AI/backp_t_en/backprop.html
// Implementation follows -> https://machinelearningmastery.com/implement-backpropagation-algorithm-scratch-python/
