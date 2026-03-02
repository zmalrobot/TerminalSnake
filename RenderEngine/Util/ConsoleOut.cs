using TerminalSnake.Rendering;
using Pastel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TerminalSnake.Util
{
    public class ConsoleOut
    {
        ConsoleConfig _consoleConfig;
        bool _enable;
        public ConsoleOut(ConsoleConfig consoleConfig, bool enable) {
            _consoleConfig = consoleConfig;
            _enable = enable;
        }

        public void ConsoleWriteLine(string message, ConsoleColors consoleColor)
        {
            if (!_enable)
                return;

            if (String.IsNullOrEmpty(message))
                return;

            ConsoleWrite(message, consoleColor);
            Console.Write(Environment.NewLine);
        }

        public void ConsoleWrite(string message, ConsoleColors consoleColor) 
        {
            if (!_enable)
                return;

            if (String.IsNullOrEmpty(message))
                return;
            
            var color = _consoleConfig.color_mapping[consoleColor];
            var elementColored = message.Pastel(color);
            Console.Write(elementColored);
        }
    }
}
