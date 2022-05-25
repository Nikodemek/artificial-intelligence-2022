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
        //var networkReader = new NeuralNetworkFileManager<Iris>("best_network_0.51.json");
        //var dataReader = new CompleteDataReader<int>("autoencoder.csv");
        var completeData = dataReader.Read();
        var (trainingData, testingData) = completeData.CreateTrainingAndTestingData(0.8, false);
        //var network = networkReader.Read();
        var network = new NeuralNetwork<Iris>(
            default,
            completeData.DataColumns, 
            3,
            completeData.Classes);

        network.Train(
            trainingData, 
            learningRate: 0.4, 
            momentum: 0.8,
            errorAccuracy: 0.0,
            epochCount: 125,
            shuffleFlag: false,
            biasFlag: true);
        
        Console.WriteLine();
        
        var output = network.FeedForward(testingData.Data[^1]);
        for (int i = 0; i < output.Length; i++)
        {
            Console.WriteLine($"{output[i] * 100.0:n3}%");
        }
        var testResult = network.Test(testingData);

        string testResultJson = Serializer.Serialize(testResult);
        File.WriteAllText(Path.Combine(Global.BaseDataDirPath, "result.json"), testResultJson);
        
        Console.ReadLine();
    }
}
