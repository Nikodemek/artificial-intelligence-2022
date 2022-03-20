namespace BossPuzzle.Utils;

public static class Cloner
{
    public static T[] SingleArr<T>(T[] arr) where T : IConvertible
    {
        int xLength = arr.Length;

        T[] ret = new T[xLength];
        for (var i = 0; i < xLength; i++)
        {
            ret[i] = arr[i];
        }
        return ret;
    }

    public static T[][] DoubleArr<T>(T[][] arr) where T : IConvertible
    {
        int xLength = arr.Length;

        T[][] ret = new T[xLength][];
        for (var i = 0; i < xLength; i++)
        {
            int yLength = arr[i].Length;
            ret[i] = new T[yLength];

            for (var j = 0; j < yLength; j++)
            {
                ret[i][j] = arr[i][j];
            }
        }
        return ret;
    }

    public static T[] Reverse<T>(this T[] arr)
    {
        int length = arr.Length;
        for (var i = 0; i < length / 2; i++)
        {
            (arr[i], arr[length - i - 1]) = (arr[length - i - 1], arr[i]);
        }
        return arr;
    }
}
