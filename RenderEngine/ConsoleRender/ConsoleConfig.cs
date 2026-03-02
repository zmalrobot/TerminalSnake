using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalSnake.Rendering
{
    public class ConsoleConfig
    {
        public int n_column { get; set; } = 100;
        public int n_row { get; set; } = 20;
        public int target_fps { get; set; } = 10;

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
