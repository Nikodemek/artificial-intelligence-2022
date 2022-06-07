using MLP.Data;
using MLP.Data.Interfaces;
using MLP.Data.Managers;
using MLP.Util;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace MLP.Model;

public class NeuralNetwork<T> where T : IConvertible
{
    private const string ErrorDataFileName = "errors.txt";
    private const int BriefTestResultsThreshold = 500;

    public NeuronLayer[] Layers { get; init; }

    private readonly Random _random = new();
    private readonly ActivationFunction _activationFunction;
    private readonly Func<int, T> _converter;
    private readonly TypeCode _typeCode;
    private readonly EqualityComparer<T> _comparer;

    public NeuralNetwork()
    {
        Layers = Array.Empty<NeuronLayer>();
        _activationFunction = Functions.SigmoidUnipolar;
        _typeCode = Type.GetTypeCode(typeof(T));
        _converter = IntToTypeDefaultConverter;
        _comparer = EqualityComparer<T>.Default;
    }

    public NeuralNetwork(ActivationFunction? activationFunction = default, params int[] neuronsInLayer)
        : this()
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

        _activationFunction = activationFunction ?? _activationFunction;
    }

    public NeuralNetwork(NeuronLayer[] layers, ActivationFunction? activationFunction = default, Func<int, T>? intToTypeConverter = default)
        : this()
    {
        Layers = layers;

        _activationFunction = activationFunction ?? _activationFunction;
        _converter = intToTypeConverter ?? IntToTypeDefaultConverter;
    }

    public void Train(DataSet<T> data, double learningRate, int epochCount = 0, double errorAccuracy = 0,
        double momentum = 0.0, bool shuffleFlag = false, bool biasFlag = true)
    {
        if (epochCount <= 0 && errorAccuracy <= 0) return;

        var errorDao = new PlainDataFileManager(ErrorDataFileName);
        var stringBuilder = new StringBuilder();

        string bestNetworkSerialized = String.Empty;

        double minError = Double.MaxValue;
        int minErrorEpoch = 0;
        Console.WriteLine($"Neural Network ({String.Join('-', Layers.Select(l => l.Neurons.Length))}): learning rate: {learningRate}, momentum: {momentum}, bias: {biasFlag}, shuffle: {shuffleFlag}");
        var watch = Stopwatch.StartNew();
        for (int i = 0, lastImprovement = 0; epochCount == 0 || i < epochCount; i++, lastImprovement++)
        {
            if (errorAccuracy > 0) if (errorAccuracy >= minError || lastImprovement > 100) break;

            if (shuffleFlag) data.Shuffle();

            double error = 0.0;
            for (var j = 0; j < data.Length; j++)
            {
                int resultVectorIndex = data.Results[j].ToInt32(NumberFormatInfo.InvariantInfo);
                double[] expected = data.GetResultVector(resultVectorIndex);

                double[] output = FeedForward(data.Data[j], biasFlag);
                BackPropagateErrors(expected);
                UpdateWeights(learningRate, momentum);
                for (var k = 0; k < expected.Length; k++)
                {
                    double diff = expected[k] - output[k];
                    error += diff * diff;
                }
            }

            stringBuilder.AppendLine(error.ToString(CultureInfo.InvariantCulture));

            if (error < minError)
            {
                minError = error;
                minErrorEpoch = i;

                bestNetworkSerialized = Serializer.Serialize(this);

                lastImprovement = 0;
            }

            Console.WriteLine($"Epoch = {i}: error = {error,-9:g5} (took {watch.ElapsedMilliseconds}ms)");
            watch.Restart();
        }
        Console.WriteLine($"Min error = {minError} occured in epoch {minErrorEpoch}");

        errorDao.Write(stringBuilder.ToString());
        var bestNetworkDao = new PlainDataFileManager($"best_network_{minError.ToString("n2", NumberFormatInfo.InvariantInfo)}");
        bestNetworkDao.Write(bestNetworkSerialized);
    }

    public ITestResult<T> Test(DataSet<T> testingData, bool biasFlag = true)
    {
        int length = testingData.Length;
        int outputLayerLength = Layers[^1].Neurons.Length;

        int correct = 0;
        var classCorrect = new Dictionary<T, int[]>();
        var actualResults = new T[length];
        var templateEntireError = new double[length];
        var templateIndividualErrors = new double[length][];

        for (int i = 0; i < length; i++)
        {
            int resultVectorIndex = testingData.Results[i].ToInt32(NumberFormatInfo.InvariantInfo);
            templateIndividualErrors[i] = new double[outputLayerLength];

            double[] output = FeedForward(testingData.Data[i], biasFlag);
            double[] expected = testingData.GetResultVector(resultVectorIndex);
            double error = 0.0;
            int maxIndex = 0;
            for (int j = 0; j < output.Length; j++)
            {
                if (output[j] > output[maxIndex]) maxIndex = j;

                double diff = expected[j] - output[j];
                double individualError = diff * diff;
                templateIndividualErrors[i][j] = individualError;

                error += individualError;
            }

            templateEntireError[i] = error;

            T result = _converter(maxIndex);
            if (_comparer.Equals(result, testingData.Results[i]))
            {
                if (classCorrect.ContainsKey(result))
                {
                    classCorrect[result][0] += 1;
                    classCorrect[result][1] += 1;
                }
                else
                {
                    classCorrect.Add(result, new[] { 1, 1 });
                }
                correct++;
            }
            else
            {
                if (classCorrect.ContainsKey(result))
                {
                    classCorrect[result][1] += 1;
                }
                else
                {
                    classCorrect.Add(result, new[] { 0, 1 });
                }
            }

            actualResults[i] = result;
        }

        var (hiddenNeuronsValues, hiddenNeuronsWeights) = HiddenLayersStats();

        var guessingClassesAccuracy = new Dictionary<T, double>();
        foreach (var (key, value) in classCorrect)
        {
            guessingClassesAccuracy.Add(key, (double)value[0] / value[1]);
        }
        double guessingAccuracy = (double)correct / (double)length;

        if (testingData.Length > NeuralNetwork<T>.BriefTestResultsThreshold)
        {
            return new BriefTestResult<T>(
                guessingAccuracy,
                guessingClassesAccuracy);
        }
        else
        {
            return new FullTestResult<T>(
                testingData.Data,
                testingData.Results,
                actualResults,
                guessingAccuracy,
                guessingClassesAccuracy,
                templateEntireError,
                templateIndividualErrors,
                Layers[^1].Neurons.Select(n => n.InputWeights).ToArray(),
                hiddenNeuronsValues,
                hiddenNeuronsWeights
                );
        }
    }

    public double[] FeedForward(double[] inputs, bool biasFlag = true)
    {
        var firstLayer = Layers[0];
        for (var i = 0; i < inputs.Length; i++)
        {
            firstLayer.Neurons[i].Value = inputs[i];
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

    private (double[][], double[][][]) HiddenLayersStats()
    {
        int hiddenLayersCount = Layers.Length - 2;
        double[][] hiddenNeuronsValues = new double[hiddenLayersCount][];
        double[][][] hiddenNeuronsWeights = new double[hiddenLayersCount][][];

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
            hiddenNeuronsValues[i - 1] = neuronsValues;
            hiddenNeuronsWeights[i - 1] = neuronsWeights;
        }

        return (hiddenNeuronsValues, hiddenNeuronsWeights);
    }

    private T IntToTypeDefaultConverter(int s)
    {
        return (T)Convert.ChangeType(s, _typeCode);
    }
}

// Help materials:
// --> https://home.agh.edu.pl/~vlsi/AI/backp_t_en/backprop.html
// Implementation follows -> https://machinelearningmastery.com/implement-backpropagation-algorithm-scratch-python/
