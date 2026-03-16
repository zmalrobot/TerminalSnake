using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalSnake.Rendering
{
    public class ConsoleConfig
    {
        // Lines consumed outside the game grid: 1 score/header line.
        private const int HeaderOverhead = 1;

        public int n_column { get; set; }
        public int n_row    { get; set; }
        public int target_fps { get; set; } = 10;

        public ConsoleConfig()
        {
            try
            {
                // Leave 1 column so the rightmost char never causes an unwanted wrap.
                // Leave 1 extra row as a scroll buffer.
                n_column = Math.Max(30, Console.WindowWidth  - 1);
                n_row    = Math.Max(10, Console.WindowHeight - HeaderOverhead - 1);
            }
            catch
            {
                // Fallback when terminal size is unavailable (e.g. redirected I/O).
                n_column = 100;
                n_row    = 20;
            }
        }

        public Dictionary<ConsoleColors, string> color_mapping = new Dictionary<ConsoleColors, string>()
        {
            { ConsoleColors.Black, "#000000"},
            { ConsoleColors.White, "#FFFFFF"},
            { ConsoleColors.Red, "#FF0000"},
            { ConsoleColors.Green, "#00FF00"},
            { ConsoleColors.Blue, "#0000FF"}
        };

    }

    public enum ConsoleColors
    {
        Black = 0,
        Red = 50,
        Green = 100,
        Blue = 150,
        White = 255,
    }

    
}
