using Serilog;
using System.Diagnostics;

namespace Игра__Быки_и_кооровы_
{
    public class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.GamePlay();
        }
    }
}