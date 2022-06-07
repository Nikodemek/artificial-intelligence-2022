using MLP.Data.Interfaces;

namespace MLP.Data;

public class MnistDataReader : IFileReader<DataSet<int>>
{
    private readonly string _filePath;
    private readonly int _dataLength;

    private const int PixelCount = 784;

    public MnistDataReader(string fileName, bool trainingDataFlag)
    {
        if (String.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("FileName cannot be empty!", nameof(fileName));

        _filePath = Path.Combine(Global.BaseDataDirPath, fileName);

        if (!File.Exists(_filePath)) throw new FileNotFoundException("File not found!", _filePath);

        _dataLength = trainingDataFlag ? 60_000 : 10_000;
    }

    public DataSet<int> Read()
    {
        FileStream fsImages = new FileStream(_filePath, FileMode.Open);
        FileStream fsLabels = new FileStream(_filePath.Replace("images", "labels").Replace("idx3", "idx1"), FileMode.Open);

        BinaryReader brImages = new BinaryReader(fsImages);
        BinaryReader brLabels = new BinaryReader(fsLabels);
        
        var datas = new List<double[]>();
        var results = new List<int>();

        // discard these
        for (var i = 0; i < 6; i++)
        {
            _ = i < 4 ? brImages.ReadInt32() : brLabels.ReadInt32();
        }

        double[] pixels = new double[PixelCount];

        for (var i = 0; i < _dataLength; i++)
        {
            for (var j = 0; j < PixelCount; j++)
            {
                byte b = brImages.ReadByte();
                pixels[j] = b; //* (1.0 / 255.0);
            }

            int result = brLabels.ReadByte();

            datas.Add(pixels);
            results.Add(result);
        }

        return new DataSet<int>(datas.ToArray(), results.ToArray());
    }
}