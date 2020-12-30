using System;

namespace Wall
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Wall())
                game.Run();
        }
    }
}
