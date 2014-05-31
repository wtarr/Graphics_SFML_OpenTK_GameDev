using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

namespace SFMLStarter2
{
    class Program
    {

        static void Main(string[] args)
        {
            Game game = new Game();
            game.Run();
        }
    }
}
