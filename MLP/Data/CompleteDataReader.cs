using MLP.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP.Data;

public class CompleteDataReader<T> : IFileReader<CompleteData<T>> where T : struct, IConvertible
{
    private readonly string _fileName;
    private readonly string _filePath;
    private readonly Func<string, T> _converter;
    private readonly TypeCode _typeCode;

    public CompleteDataReader(string fileName, Func<string, T>? converter = default)
    {
        if (String.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("FileName cannot be empty!", nameof(fileName));
        if (!fileName.EndsWith(".csv")) throw new ArgumentException("File must b of type .csv", nameof(fileName));

        _fileName = fileName;
        _filePath = Path.Combine(Global.BaseDataDirPath, _fileName);

        if (!File.Exists(_filePath)) throw new FileNotFoundException("File not found!", _filePath);

        _typeCode = Type.GetTypeCode(typeof(T));
        _converter = converter ?? DefaultConverter;
    }

    public CompleteData<T> Read()
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

        return new CompleteData<T>(datas.ToArray(), results.ToArray());
    }

    private T DefaultConverter(string s)
    {
        return (T)Convert.ChangeType(s, _typeCode);
    }
}