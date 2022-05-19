using MLP.Data;
using System.Text;

namespace MLP;

public static class Program
{
    public static void Main()
    {
        Global.EnsureDirectoryIsValid(true);

        var fileReader = new IrisesTrainingDataReader("data.csv");
        var trainingData = fileReader.Read();

        for (int i = 0; i < trainingData.Length; i++)
        {
            var sb = new StringBuilder();

            for (int j = 0; j < trainingData.DataColumns; j++)
            {
                sb.Append(trainingData.Data[i, j]).Append('\t');
            }
            sb.Append(" --> ").Append(trainingData.Results[i]);
            Console.WriteLine(sb);
        }

        Console.ReadLine();
    }
}