using MLP.Data;
using MLP.Model;

namespace MLP;

public static class Program
{
    public static void Main()
    {
        Global.EnsureDirectoryIsValid();

        var dataReader = new CompleteDataReader<Iris>("data.csv");
        //var dataReader = new CompleteDataReader<int>("autoencoder.csv");
        var completeData = dataReader.Read();
        var (trainingData, testingData) = completeData.CreateTrainingAndTestingData(0.8, false);

        var network = new NeuralNetwork(default, 4, 4, 3);
        var output = network.FeedForward(trainingData.Data[0]);
        foreach (var d in output)
        {
            Console.WriteLine(d);
        }

        Console.WriteLine();
        network.Train(completeData, 0.1, errorAccuracy: 0.7, shuffleFlag: false);
        output = network.FeedForward(trainingData.Data[^1]);
        for (int i = 0; i < output.Length; i++)
        {
            Console.WriteLine($"{output[i] * 100.0:n3}%");
        }
        network.Test(completeData);

        /*var networkFileManager = new NeuralNetworkFileManager("network.txt");
        var network = networkFileManager.Read();
        networkFileManager.Write(network);*/

        Console.ReadLine();
    }
}