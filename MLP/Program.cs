using MLP.Data;
using MLP.Model;
using MLP.Util;
using MLP.Data.Managers;

namespace MLP;

public static class Program
{
    public static void Main()
    {
        Global.EnsureDirectoryIsValid();

        var MnistTrainingDataReader = new MnistDataReader("train-images.idx3-ubyte");
        var trainingData = MnistTrainingDataReader.Read();
        
        //var dataReader = new CompleteDataReader<Iris>("data.csv");
        //var networkReader = new NeuralNetworkFileManager<Iris>("best_network_0.51.json");
        //var dataReader = new CsvDataReader<int>("autoencoder.csv");
        //var completeData = dataReader.Read();
        //var (trainingData, testingData) = completeData.CreateTrainingAndTestingData(0.8, false);
        //var network = networkReader.Read();

        var network = new NeuralNetwork<int>(
            Functions.SigmoidUnipolar,
            trainingData.DataColumns, 
            120,
            84,
            trainingData.Classes);

        network.Train(
            trainingData,
            learningRate: 0.4,
            momentum: 0.9,
            errorAccuracy: 0.0,
            epochCount: 15,
            shuffleFlag: true,
            biasFlag: true);
        
        Console.WriteLine();
        var output = network.FeedForward(trainingData.Data[^1]);
        for (int i = 0; i < output.Length; i++)
        {
            Console.WriteLine($"{output[i] * 100.0:n3}%");
        }
        var testResult = network.Test(trainingData);

        string testResultJson = Serializer.Serialize(testResult);
        File.WriteAllText(Path.Combine(Global.BaseDataDirPath, "result.json"), testResultJson);
        
        Console.ReadLine();
    }
}
