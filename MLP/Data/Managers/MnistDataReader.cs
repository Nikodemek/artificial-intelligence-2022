using MLP.Data.Interfaces;
using MLP.Util;

namespace MLP.Data.Managers;

public class MnistDataReader : IFileReader<DataSet<int>>
{
    private const int MaxData = Int32.MaxValue;
    private readonly string _filePath;

    public MnistDataReader(string fileName, bool trainingDataFlag)
    {
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("FileName cannot be empty!", nameof(fileName));

        _filePath = Path.Combine(Global.BaseDataDirPath, fileName);

        if (!File.Exists(_filePath)) throw new FileNotFoundException("File not found!", _filePath);
    }

    public DataSet<int> Read()
    {
        using var imagesFs = new FileStream(_filePath, FileMode.Open);
        using var labelsFs = new FileStream(_filePath.Replace("images", "labels").Replace("idx3", "idx1"), FileMode.Open);

        using var imagesBr = new BinaryReader(imagesFs);
        using var labelsBr = new BinaryReader(labelsFs);

        int imagesMagicNumber = imagesBr.ReadInt32(true);
        int imagesCount = imagesBr.ReadInt32(true);
        int imagesRows = imagesBr.ReadInt32(true);
        int imagesColumns = imagesBr.ReadInt32(true);

        int labelsMagicNumber = labelsBr.ReadInt32(true);
        int labelsCount = labelsBr.ReadInt32(true);

        int dataCount = imagesCount == labelsCount ? Math.Min(imagesCount, MaxData) : throw new ArgumentException("Data set and result set must be equal!"); ;
        int pixelsCount = imagesRows * imagesColumns;

        var datas = new double[dataCount][];
        var results = new int[dataCount];

        for (var i = 0; i < dataCount; i++)
        {
            var data = new double[pixelsCount];
            for (var j = 0; j < pixelsCount; j++)
            {
                data[j] = imagesBr.ReadByte() / 255.0;
            }
            datas[i] = data;
            results[i] = labelsBr.ReadByte();
        }

        return new DataSet<int>(datas, results);
    }
}