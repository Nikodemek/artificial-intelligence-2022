using MLP.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP.Data;

public class IrisesTrainingDataReader : IFileReader<ITrainingData<double, IrisType>>
{
    private readonly string _fileName;
    private readonly string _filePath;

    public IrisesTrainingDataReader(string fileName)
    {
        if (String.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("FileName cannot be empty!", nameof(fileName));
        if (!fileName.EndsWith(".csv")) throw new ArgumentException("File must b of type .csv", nameof(fileName));

        _fileName = fileName;
        _filePath = Path.Combine(Global.BaseDataDirPath, _fileName);

        if (!File.Exists(_filePath)) throw new FileNotFoundException("File not found!", _filePath);
    }

    public ITrainingData<double, IrisType> Read()
    {
        string[] fileData = File.ReadAllLines(_filePath);

        var datas = new List<double[]>(fileData.Length);
        var results = new List<IrisType>(fileData.Length);

        foreach (string line in fileData)
        {
            var lineData = line.Split(',');
            if (lineData.Length < 2) break;

            var data = new double[lineData.Length - 1];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = lineData[i].ToDouble();
            }
            IrisType result = lineData[^1].ToIrisType();

            datas.Add(data);
            results.Add(result);
        }

        return new IrisesTrainingData(datas.To2DArray(), results.ToArray());
    }
}
