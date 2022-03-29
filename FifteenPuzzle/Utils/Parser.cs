using System.Globalization;

namespace FifteenPuzzle.Utils;

public static class Parser
{
    public static int ToInt32(string s)
    {
        return Int32.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
    }

    public static short ToInt16(string s)
    {
        return Int16.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
    }
}
