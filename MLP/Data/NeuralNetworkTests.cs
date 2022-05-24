using MLP.Model;

namespace MLP.Data;

public static class NeuralNetworkTests<T> where T : IConvertible
{
    public static void FindBestNetworkOverall(DataSet<T> data, double dataTrainingPart = 0.8, int caseCount = 10, int epochCount = 300, bool shuffle = false, params int[] neuronsInLayer)
    {
        var (trainingData, _) = data.CreateTrainingAndTestingData(dataTrainingPart, shuffle);

        for (var i = 0.1; i <= 1; i += .1)
        {
            for (var j = 0.1; j <= 1; j += .1)
            {
                for (var k = 0; k < caseCount; k++)
                {
                    var network = new NeuralNetwork<T>(default, neuronsInLayer);
                    network.Train(trainingData, i, epochCount, momentum: j, shuffleFlag: shuffle);
                }
            }
            string[] files = Directory.GetFiles(Global.BaseDataDirPath, "best_network*");
            for (var j = files.Length - 1; j > 0; j--)
            {
                File.Delete(files[j]);
            }
        }
    }
}