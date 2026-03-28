using Spectre.Console;
using System;

namespace TerminalSnake.Util
{
    /// <summary>
    /// <see cref="IConsoleOutput"/> implementation backed by Spectre.Console.
    /// UI methods (Write, WriteLine, WriteAt, ResetColor) route through
    /// Spectre's styled rendering pipeline, giving rich 24-bit colour,
    /// proper markup, and cross-platform compatibility.
    ///
    /// <see cref="WriteRaw"/> writes pre-formatted ANSI directly to stdout so
    /// that the high-frequency game render loop bypasses Spectre's overhead.
    /// </summary>
    public class ConsoleOutSpectre : IConsoleOutput
    {
        public void Write(string text)
            => AnsiConsole.Write(text);

        public void WriteLine(string text)
            => AnsiConsole.WriteLine(text);

        public void Write(string text, ConsoleColor color)
            => AnsiConsole.Write(new Text(text, new Style(ToSpectreColor(color))));

        public void WriteLine(string text, ConsoleColor color)
        {
            AnsiConsole.Write(new Text(text + Environment.NewLine,
                                       new Style(ToSpectreColor(color))));
        }

        public void WriteAt(int row, int col, string text, ConsoleColor color)
        {
            // Position cursor with raw ANSI, then let Spectre render the styled text.
            Console.Write($"\x1B[{row};{col}H");
            AnsiConsole.Write(new Text(text, new Style(ToSpectreColor(color))));
        }

        /// <summary>
        /// Pass a pre-built ANSI frame string directly to stdout.
        /// Used by the game loop so Spectre's per-call overhead is avoided.
        /// </summary>
        public void WriteRaw(string raw)
            => Console.Write(raw);

        public void Clear()
            => Console.Write("\x1B[2J\x1B[H");

        public void SetCursorVisible(bool visible)
            => Console.CursorVisible = visible;

        public void ResetColor()
            => AnsiConsole.Reset();

        // ── Colour mapping ───────────────────────────────────────────────────────

        private static Spectre.Console.Color ToSpectreColor(ConsoleColor c) => c switch
        {
            ConsoleColor.Black       => Spectre.Console.Color.Black,
            ConsoleColor.DarkBlue    => Spectre.Console.Color.NavyBlue,
            ConsoleColor.DarkGreen   => Spectre.Console.Color.DarkGreen,
            ConsoleColor.DarkCyan    => Spectre.Console.Color.Teal,
            ConsoleColor.DarkRed     => Spectre.Console.Color.Maroon,
            ConsoleColor.DarkMagenta => Spectre.Console.Color.Purple,
            ConsoleColor.DarkYellow  => Spectre.Console.Color.Olive,
            ConsoleColor.Gray        => Spectre.Console.Color.Silver,
            ConsoleColor.DarkGray    => Spectre.Console.Color.Grey,
            ConsoleColor.Blue        => Spectre.Console.Color.Blue,
            ConsoleColor.Green       => Spectre.Console.Color.Lime,
            ConsoleColor.Cyan        => Spectre.Console.Color.Aqua,
            ConsoleColor.Red         => Spectre.Console.Color.Red,
            ConsoleColor.Magenta     => Spectre.Console.Color.Fuchsia,
            ConsoleColor.Yellow      => Spectre.Console.Color.Yellow,
            ConsoleColor.White       => Spectre.Console.Color.White,
            _                        => Spectre.Console.Color.White,
        };
    }
}
