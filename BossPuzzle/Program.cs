using System;
using BossPuzzle.Dao;
using BossPuzzle.PuzzleBoard;

namespace BossPuzzle;
using Dir = Board.Direction;

class Program
{
    public static void Main()
    {
        var sth = new FileFifteenPuzzleDao("test.file");
        Board board = sth.Read();
        board.Print();
        Console.WriteLine(board.IsValid());

        var board1 = board.Move(2, 2, Dir.Up);
        board1.Print();

        var board2 = board1.Move(1, 2, Dir.Down);
        board2.Print();

        var board3 = board1.Move(Dir.Up);
        board3.Print();

        var board4 = board3.Move(Dir.Left);
        board4.Print();

        var board5 = board.Move(Dir.Up).Move(Dir.Left).Move(Dir.Down).Move(Dir.Right);
        board5.Print();

        Console.WriteLine($"board2 == board = {board2 == board}  //Expected 'true'");
        Console.WriteLine($"board5 == board = {board5 == board}  //Expected 'false'");

        Console.ReadKey();
    }
}