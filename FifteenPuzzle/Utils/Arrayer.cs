namespace FifteenPuzzle.Utils;

public static class Arrayer
{
    public static T[] Copy<T>(T[] arr) where T : IConvertible
    {
        int xLength = arr.Length;

        var ret = new T[xLength];
        for (var i = 0; i < xLength; i++)
        {
            ret[i] = arr[i];
        }
        return ret;
    }

    public static T[,] Copy<T>(T[,] arr) where T : IConvertible
    {
        int xLength = arr.GetLength(0);
        int yLength = arr.GetLength(1);

        var ret = new T[xLength, yLength];
        for (var i = 0; i < xLength; i++)
        {
            for (var j = 0; j < yLength; j++)
            {
                ret[i, j] = arr[i, j];
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

    public static T[] Shuffle<T>(this T[] arr)
    {
        var rand = new Random();
        int length = arr.Length;

        for (var i = 0; i < length; i++)
        {
            int toSwap = rand.Next(0, length - 1);
            (arr[toSwap], arr[i]) = (arr[i], arr[toSwap]);
        }

        return arr;
    }
}
