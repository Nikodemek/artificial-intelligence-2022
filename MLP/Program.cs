using MLP.Data;
using System.Text;
using MLP.Model;

namespace MLP;

public static class Program
{
    public static void Main()
    {
        Global.EnsureDirectoryIsValid(true);

        var fileReader = new IrisesTrainingDataReader("data.csv");
        var trainingData = fileReader.Read();

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

        NeuralNetwork network = new NeuralNetwork(default, 4, 3, 3, 3);
        var output = network.FeedForward(trainingData.Data[0]);
        foreach (var d in output)
        {
            Console.WriteLine(d);
        }

        Console.WriteLine();
        network.Train(trainingData, 0.1, 1000);
        output = network.FeedForward(trainingData.Data[101]);
        foreach (var d in output)
        {
            Console.WriteLine(d);
        }

        Console.WriteLine();

        /*var networkFileManager = new NeuralNetworkFileManager("network.txt");
        var network = networkFileManager.Read();
        networkFileManager.Write(network);*/

        Console.ReadLine();
    }
}