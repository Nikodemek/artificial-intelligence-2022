using MLP.Data.Interfaces;
using MLP.Model;
using MLP.Util;
using System.Text;

namespace MLP.Data.Managers;

public class NeuralNetworkFileManager<T> : IFileManager<NeuralNetwork<T>> where T : IConvertible
{
    public string FileName { get; set; }
    private string FilePath => Path.Combine(Global.BaseDataDirPath, FileName.Contains(".json") ? FileName : FileName + ".json");

    public NeuralNetworkFileManager(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("Filename cannot be empty!", nameof(fileName));

        FileName = fileName;
    }

    public NeuralNetwork<T> Read()
    {
        string fileData = File.ReadAllText(FilePath);
        return Serializer.Deserialize<NeuralNetwork<T>>(fileData);
    }

    public void Write(NeuralNetwork<T> network)
    {
        string data = Serializer.Serialize(network);
        File.WriteAllText(FilePath, data);
    }
}