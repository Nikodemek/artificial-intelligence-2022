using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP;

public static class Utils
{
    private static readonly Random rand = new();

    public static double ToDouble(this string number)
    {
        if (double.TryParse(number, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out double value)) return value;
        else return -1.0;
    }

    public static int ToInt32(this string number)
    {
        if (int.TryParse(number, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out int value)) return value;
        else return -1;
    }

    public static IrisType ToIrisType(this string number)
    {
        int value = number.ToInt32();
        if (value.NotBetween(0, 2)) throw new Exception($"Wrong Iris with flag = {value}");
        return (IrisType)value;
    }

    public static bool Between(this int value, int min, int max)
    {
        return value >= min && value <= max;
    }

    public static bool NotBetween(this int value, int min, int max)
    {
        return value < min || value > max;
    }

    public static T[,] To2DArray<T>(this List<T[]> doublesList)
    {
        int rowLength = doublesList.Count;
        int columnLength = doublesList[0].Length;

        var newArray = new T[rowLength, columnLength];

        for (var i = 0; i < rowLength; i++)
        {
            var arr = doublesList[i];
            if (arr.Length != columnLength) throw new ArgumentException("List of arrays is not rectangular!", nameof(doublesList));

            for (int j = 0; j < columnLength; j++)
            {
                newArray[i, j] = arr[j];
            }
        }

        return newArray;
    }

    public static int CountUnique<T>(this IEnumerable<T> collection)
    {
        var set = new HashSet<T>(collection);
        return set.Count;
    }

    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        int count = list.Count;
        for (int i = 0; i < count; i++)
        {
            int index = rand.Next(count);
            (list[i], list[index]) = (list[index], list[i]);
        }
        return list;
    }
}
