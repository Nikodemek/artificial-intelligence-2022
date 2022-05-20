using MLP.Data.Interfaces;
using MLP.Model;
using System.Text;

namespace MLP.Data;

public class NeuralNetworkFileManager : IFileManager<NeuralNetwork>
{
    private readonly string _fileName;
    private readonly string _filePath;

    public NeuralNetworkFileManager(string fileName)
    {
        if (String.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("FileName cannot be empty!", nameof(fileName));

        _fileName = fileName;
        _filePath = Path.Combine(Global.BaseDataDirPath, _fileName);
    }

    public NeuralNetwork Read()
    {
        var fileData = File.ReadAllLines(_filePath);
        if (fileData.Length < 1) return new NeuralNetwork();
        int offset = 0;

        int layersCount = fileData[offset++].Trim().ToInt32();
        var layers = new NeuronLayer[layersCount];
        for (int i = 0; i < layersCount; i++)
        {
            int neuronsCount = fileData[offset++].Trim().ToInt32();
            var neurons = new Neuron[neuronsCount];
            for (int j = 0; j < neuronsCount; j++)
            {
                var weightsString = fileData[offset++].Trim().Split(' ');
                int weightsCount = weightsString.Length;
                var weights = new double[weightsCount];
                for (int k = 0; k < weightsCount; k++)
                {
                    weights[k] = weightsString[k].ToDouble();
                }
                double bias = fileData[offset++].Trim().ToDouble();
                double value = fileData[offset++].Trim().ToDouble();

                var neuron = new Neuron(weights)
                {
                    Bias = bias,
                    Value = value,
                };
                neurons[j] = neuron;
            }

            var layer = new NeuronLayer(neurons);
            layers[i] = layer;
        }

        var neuralNetwork = new NeuralNetwork(layers);
        return neuralNetwork;
    }

    public void Write(NeuralNetwork network)
    {
        if (!File.Exists(_filePath)) throw new FileNotFoundException("File not found!", _filePath);
        var sb = new StringBuilder(1000);

        var layers = network.Layers;
        sb.Append('\t', 0).Append(layers.Length).AppendLine();
        foreach (var layer in layers)
        {
            var neurons = layer.Neurons;
            sb.Append('\t', 1).Append(neurons.Length).AppendLine();
            foreach (var neuron in neurons)
            {
                var weights = neuron.InputWeights;
                double bias = neuron.Bias;
                double value = neuron.Value;
                sb.Append('\t', 2).AppendJoin(' ', weights).AppendLine();
                sb.Append('\t', 2).Append(bias).AppendLine();
                sb.Append('\t', 2).Append(value).AppendLine();
            }
        }

        File.WriteAllText(_filePath, sb.ToString());
    }
}