using System;
using BossPuzzle.Dao;
using BossPuzzle.PuzzleBoard;

namespace BossPuzzle;

class Program
{
    public static void Main()
    {
        var sth = new FileFifteenPuzzleDao("test.file");
        Board board = sth.Read();
        board.Print();
        Console.WriteLine(board.IsValid());

        Console.ReadKey();
    }
}