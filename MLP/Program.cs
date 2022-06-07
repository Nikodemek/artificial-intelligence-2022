using MLP.Data;
using MLP.Data.Managers;
using MLP.Model;
using MLP.Util;

namespace MLP;

public static class Program
{
    public static void Main()
    {
        Global.EnsureDirectoryIsValid();

        var mnistTrainingDataReader = new MnistDataReader("train-images.idx3-ubyte");
        var mnistTestDataReader = new MnistDataReader("t10k-images.idx3-ubyte");
        var trainingData = mnistTrainingDataReader.Read();
        var testData = mnistTestDataReader.Read();

        //var dataReader = new CompleteDataReader<Iris>("data.csv");
        //var networkReader = new NeuralNetworkFileManager<Iris>("best_network_0.51.json");
        //var dataReader = new CsvDataReader<int>("autoencoder.csv");
        //var completeData = dataReader.Read();
        //var (trainingData, testingData) = completeData.CreateTrainingAndTestingData(0.8, false);
        //var network = networkReader.Read();

        var network = new NeuralNetwork<int>(
            Functions.SigmoidUnipolar,
            trainingData.DataColumns, 
            16,
            trainingData.Classes);

        network.Train(
            trainingData,
            learningRate: 0.4,
            momentum: 0.9,
            errorAccuracy: 0.0,
            epochCount: 2,
            shuffleFlag: true,
            biasFlag: true);

        Console.WriteLine();
        var output = network.FeedForward(testData.Data[^1]);
        for (int i = 0; i < output.Length; i++)
        {
            Console.WriteLine($"{i}: {output[i] * 100.0:n3}%");
        }
        
        Console.WriteLine($"Expected prediction: {testData.Results[^1]}");
        
        var testResult = network.Test(testData);
        string testResultJson = Serializer.Serialize(testResult);

        var testResultDataManager = new PlainDataFileManager("result.json");
        testResultDataManager.Write(testResultJson);

        Console.ReadLine();
    }
}
