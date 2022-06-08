using System.Diagnostics;
using System.Globalization;
using MLP.Data;
using MLP.Data.Managers;
using MLP.Model;
using MLP.Util;

namespace MLP;

public static class Program
{
    public static void Main(string[] args)
    {
        Global.EnsureDirectoryIsValid();

        /*var dataReader = new CsvDataReader<Iris>("data.csv");
        var dataReader = new CsvDataReader<int>("autoencoder.csv");
        var completeData = dataReader.Read();
        var (trainingData, testingData) = completeData.CreateTrainingAndTestingData(0.8, false);
        
        var networkReader = new NeuralNetworkFileManager<Iris>("best_network_0.51.json");
        var network = networkReader.Read();*/

        var mnistTrainingDataReader = new MnistDataReader("train-images.idx3-ubyte");
        var mnistTestDataReader = new MnistDataReader("t10k-images.idx3-ubyte");
        var trainingData = mnistTrainingDataReader.Read();
        var testData = mnistTestDataReader.Read();

        var (epochCount, networkLayers) = ParseInput(args, trainingData);

        var network = new NeuralNetwork<int>(
            Functions.SigmoidUnipolar,
            networkLayers);

        Task.Run(delegate
        {
            Thread.Sleep(500);
            while (network.IsTraining)
            {
                char read = Console.ReadKey().KeyChar;
                if (read is 'q' or 'Q')
                {
                    network.Interrupt();
                    return;
                }
            }
        });

        network.Train(
            trainingData,
            learningRate: 0.3,
            momentum: 0.8,
            errorAccuracy: 0.0,
            epochCount: epochCount,
            shuffleFlag: true,
            biasFlag: true);

        Console.WriteLine();
        var output = network.FeedForward(testData.Data[^1]);
        for (int i = 0; i < output.Length; i++)
        {
            string correct = i == testData.Results[^1] ? "<--" : String.Empty;
            Console.WriteLine($"{i}: {output[i] * 100.0:n3}% {correct}");
        }
        
        var testResult = network.Test(testData);
        string testResultJson = Serializer.Serialize(testResult);

        var testResultDataManager = new PlainDataFileManager("result.json");
        testResultDataManager.Write(testResultJson);

        Console.ReadLine();
    }
    
    private static (int, int[]) ParseInput<T>(string[] args, DataSet<T> data)
    {
        int argsLength = args.Length;
        
        int epochCount = 10;
        if (argsLength > 0) epochCount = args[0].ToInt32();
        
        int[] networkLayers = new[] { data.DataColumns, 32, data.Classes };
        if (argsLength > 1)
        {
            var countsString = args[1].Split('-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            networkLayers = new int[countsString.Length + 2];
            networkLayers[0] = data.DataColumns;
            for (int i = 0; i < countsString.Length; i++)
            {
                networkLayers[i + 1] = countsString[i].ToInt32();
            }
            networkLayers[^1] = data.Classes;
        }
        
        return (epochCount, networkLayers);
    }
}
