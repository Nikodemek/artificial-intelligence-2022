using MLP.Data;
using System.Globalization;

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

    public static Iris ToIrisType(this string number)
    {
        int value = number.ToInt32();
        if (value.NotBetween(0, 2)) throw new Exception($"Wrong Iris with flag = {value}");
        return (Iris)value;
    }

    public static bool Between(this int value, int min, int max) => value >= min && value <= max;
    public static bool NotBetween(this int value, int min, int max) => value < min || value > max;
    public static bool Between(this double value, double min, double max) => value >= min && value <= max;
    public static bool NotBetween(this double value, double min, double max) => value < min || value > max;

    public static T[,] To2DArray<T>(this List<T[]> doublesList)
    {
        int rowLength = doublesList.Count;
        int columnLength = doublesList[0].Length;

        var newArray = new T[rowLength, columnLength];

        for (var i = 0; i < rowLength; i++)
        {
            var arr = doublesList[i];
            if (arr.Length != columnLength) throw new ArgumentException("List of arrays is not rectangular!", nameof(doublesList));

            for (var j = 0; j < columnLength; j++)
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

    // For matrices
    public static T[] GetRow<T>(this T[,] matrix, int row)
    {
        var rowLength = matrix.GetLength(1);
        var rowVector = new T[rowLength];

        for (var i = 0; i < rowLength; i++)
            rowVector[i] = matrix[row, i];

        return rowVector;
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

    public static T[] GClone<T>(this T[] arr)
    {
        return (T[])arr.Clone();
    }

    public static T[][] GClone<T>(this T[][] arr)
    {
        int length = arr.Length;
        T[][] newArr = new T[length][];
        for (int i = 0; i < length; i++)
        {
            newArr[i] = arr[i].GClone();
        }
        return newArr;
    }
}
