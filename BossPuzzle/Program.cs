using System;

namespace BossPuzzle;

class Program
{
    public static void Main()
    {
        Console.WriteLine("Hello loser");
        var sth = new FileFifteenPuzzleDao("test.file");
        sth.Read();

        Console.ReadKey();
    }
}