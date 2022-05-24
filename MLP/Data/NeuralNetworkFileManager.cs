using MLP.Data.Interfaces;
using MLP.Model;
using MLP.Util;
using System.Text;

namespace MLP.Data;

public class NeuralNetworkFileManager<T> : IFileManager<NeuralNetwork<T>> where T : IConvertible
{
    private readonly string _fileName;
    private readonly string _filePath;

    public NeuralNetworkFileManager(string fileName)
    {
        if (String.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("FileName cannot be empty!", nameof(fileName));

        _fileName = fileName;
        _filePath = Path.Combine(Global.BaseDataDirPath, _fileName);
    }

    public NeuralNetwork<T> Read()
    {
        string fileData = File.ReadAllText(_filePath);
        return Serializer.Deserialize<NeuralNetwork<T>>(fileData);
    }

    public void Write(NeuralNetwork<T> network)
    {
        if (!File.Exists(_filePath)) throw new FileNotFoundException("File not found!", _filePath);
        string data = Serializer.Serialize(network);
        File.WriteAllText(_filePath, data);
    }
}