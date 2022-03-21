using System.Globalization;

namespace BossPuzzle.Utils;

public static class Parser
{
    public static int ToInt32(string s)
    {
        return Int32.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
    }

    public static long ToInt16(string s)
    {
        return Int16.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
    }
}
