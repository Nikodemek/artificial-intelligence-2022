using MLP.Data;
using MLP.Model;
using MLP.Util;

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

        var network = new NeuralNetwork<Iris>(default, 4, 4, 3);
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
        var testResult = network.Test(completeData);

        string testResultJson = Serializer.Serialize(testResult);
        File.WriteAllText(Path.Combine(Global.BaseDataDirPath, "result.json"), testResultJson);

        /*var networkFileManager = new NeuralNetworkFileManager("network.txt");
        var network = networkFileManager.Read();
        networkFileManager.Write(network);*/

        Console.ReadLine();
    }
}