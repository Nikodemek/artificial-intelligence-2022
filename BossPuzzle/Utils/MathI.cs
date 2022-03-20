using System;

namespace BossPuzzle.Utils;

public static class MathI
{
    public static ulong Power(ulong n, int pow)
    {
        ulong result = 1;
        for (int i = 0; i < pow; i++)
        {
            result *= n;
        }
        return result;
    }

    public static int Power(int n, int pow)
    {
        int result = 1;
        for (int i = 0; i < pow; i++)
        {
            result *= n;
        }
        return result;
    }
}
