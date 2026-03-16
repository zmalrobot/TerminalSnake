using TerminalSnake.Rendering;
using Pastel;
using System;
using System.Collections.Generic;

namespace TerminalSnake.Util
{
    public class ConsoleOut : IConsoleOutput
    {
        private readonly ConsoleConfig _consoleConfig;
        private readonly bool _enable;

        // Maps System.ConsoleColor values to ANSI-compatible hex strings for Pastel.
        // Using Pastel (ANSI sequences) ensures Linux/macOS compatibility.
        private static readonly Dictionary<ConsoleColor, string> _colorHexMap = new()
        {
            [ConsoleColor.Black]       = "#000000",
            [ConsoleColor.DarkBlue]    = "#000080",
            [ConsoleColor.DarkGreen]   = "#008000",
            [ConsoleColor.DarkCyan]    = "#008080",
            [ConsoleColor.DarkRed]     = "#800000",
            [ConsoleColor.DarkMagenta] = "#800080",
            [ConsoleColor.DarkYellow]  = "#808000",
            [ConsoleColor.Gray]        = "#C0C0C0",
            [ConsoleColor.DarkGray]    = "#808080",
            [ConsoleColor.Blue]        = "#0000FF",
            [ConsoleColor.Green]       = "#00FF00",
            [ConsoleColor.Cyan]        = "#00FFFF",
            [ConsoleColor.Red]         = "#FF0000",
            [ConsoleColor.Magenta]     = "#FF00FF",
            [ConsoleColor.Yellow]      = "#FFFF00",
            [ConsoleColor.White]       = "#FFFFFF",
        };

        public ConsoleOut(ConsoleConfig consoleConfig, bool enable)
        {
            _consoleConfig = consoleConfig;
            _enable = enable;
        }

        // ── IConsoleOutput ───────────────────────────────────────────────────────

        public void Write(string text)
        {
            if (!_enable || string.IsNullOrEmpty(text)) return;
            Console.Write(text);
        }

        public void WriteLine(string text)
        {
            if (!_enable || string.IsNullOrEmpty(text)) return;
            Console.WriteLine(text);
        }

        public void Write(string text, ConsoleColor color)
        {
            if (!_enable || string.IsNullOrEmpty(text)) return;
            Console.Write(text.Pastel(_colorHexMap[color]));
        }

        public void WriteLine(string text, ConsoleColor color)
        {
            if (!_enable || string.IsNullOrEmpty(text)) return;
            Console.WriteLine(text.Pastel(_colorHexMap[color]));
        }

        public void WriteAt(int row, int col, string text, ConsoleColor color)
        {
            if (!_enable) return;
            // ANSI absolute cursor positioning: ESC[row;colH
            Console.Write($"\x1B[{row};{col}H{text.Pastel(_colorHexMap[color])}");
        }

        public void WriteRaw(string raw)
        {
            if (!_enable || string.IsNullOrEmpty(raw)) return;
            Console.Write(raw);
        }

        public void Clear()
        {
            if (!_enable) return;
            // ANSI erase-display + cursor-home. More reliable than Console.Clear()
            // in VT terminals (VS Code, Windows Terminal, xterm, Linux pty).
            Console.Write("\x1B[2J\x1B[H");
        }

        public void SetCursorVisible(bool visible)
        {
            if (!_enable) return;
            Console.CursorVisible = visible;
        }

        public void ResetColor()
        {
            if (!_enable) return;
            Console.Write("\x1B[0m");
        }

        // ── Engine-internal helpers (ConsoleColors enum) ─────────────────────────

        /// <summary>Write a colored line using the engine's ConsoleColors enum.</summary>
        public void ConsoleWriteLine(string message, ConsoleColors consoleColor)
        {
            if (!_enable || string.IsNullOrEmpty(message)) return;
            Console.WriteLine(message.Pastel(_consoleConfig.color_mapping[consoleColor]));
        }

        /// <summary>Write colored text using the engine's ConsoleColors enum.</summary>
        public void ConsoleWrite(string message, ConsoleColors consoleColor)
        {
            if (!_enable || string.IsNullOrEmpty(message)) return;
            Console.Write(message.Pastel(_consoleConfig.color_mapping[consoleColor]));
        }
    }
}
