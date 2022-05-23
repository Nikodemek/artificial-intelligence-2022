using MLP.Data;
using MLP.Model;

namespace MLP;

public static class Program
{
    public static void Main()
    {
        Global.EnsureDirectoryIsValid();

        var dataReader = new CompleteDataReader<Iris>("data.csv");
        //var fileReader = new TrainingDataReader<int>("autoencoder.csv");
        var completeData = dataReader.Read();
        var (trainingData, testingData) = completeData.CreateTrainingAndTestingData(0.8);

        /*for (int i = 0; i < trainingData.Length; i++)
        {
            var sb = new StringBuilder();
            for (int j = 0; j < trainingData.DataColumns; j++)
            {
                sb.Append(trainingData.Data[i][j]).Append('\t');
            }
            sb.Append(" --> ").Append(trainingData.Results[i]);
            Console.WriteLine(sb);
        }*/

        var network = new NeuralNetwork(default, 4, 3, 3);
        var output = network.FeedForward(trainingData.Data[0]);
        foreach (var d in output)
        {
            Console.WriteLine(d);
        }

        Console.WriteLine();
        network.Train(trainingData, 0.1, errorAccuracy: 0.7);
        output = network.FeedForward(trainingData.Data[^1]);
        for (int i = 0; i < output.Length; i++)
        {
            Console.WriteLine($"{output[i] * 100.0:n3}%");
        }

        /*var networkFileManager = new NeuralNetworkFileManager("network.txt");
        var network = networkFileManager.Read();
        networkFileManager.Write(network);*/

        Console.ReadLine();
    }
}