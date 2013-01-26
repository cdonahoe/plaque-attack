using System;

namespace PlaqueAttack
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game game = Game.GetGame())
            {
                game.Run();
            }
        }
    }
#endif
}

