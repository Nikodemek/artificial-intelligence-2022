using MLP.Data.Interfaces;
using MLP.Util;

namespace MLP.Data.Managers;

public class CsvDataReader<T> : IFileReader<DataSet<T>> where T : IConvertible
{
    private readonly string _fileName;
    private readonly string _filePath;
    private readonly Func<string, T> _converter;
    private readonly TypeCode _typeCode;

    public CsvDataReader(string fileName, Func<string, T>? converter = default)
    {
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("FileName cannot be empty!", nameof(fileName));
        if (!fileName.EndsWith(".csv")) throw new ArgumentException("File must be of type .csv", nameof(fileName));

        _fileName = fileName;
        _filePath = Path.Combine(Global.BaseDataDirPath, _fileName);

        if (!File.Exists(_filePath)) throw new FileNotFoundException("File not found!", _filePath);

        _typeCode = Type.GetTypeCode(typeof(T));
        _converter = converter ?? StringToTypeDefaultConverter;
    }

    public DataSet<T> Read()
    {
        string[] fileData = File.ReadAllLines(_filePath);

        var datas = new List<double[]>(fileData.Length);
        var results = new List<T>(fileData.Length);

        foreach (string line in fileData)
        {
            var lineData = line.Split(',');
            if (lineData.Length < 2) break;

            var data = new double[lineData.Length - 1];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = lineData[i].ToDouble();
            }
            T result = _converter(lineData[^1]);

            datas.Add(data);
            results.Add(result);
        }

        return new DataSet<T>(datas.ToArray(), results.ToArray());
    }

    private T StringToTypeDefaultConverter(string s)
    {
        return (T)Convert.ChangeType(s, _typeCode);
    }
}