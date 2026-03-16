using TerminalSnake.Util;
using TerminalSnake.Rendering;

namespace TerminalSnake
{
    public static class TitleScreen
    {
        private static readonly string[] _titleArt =
        {
            @"  ____  _   _    _   _  _  _____  ",
            @" / ___|| \ | |  / \ | |/ /| ____| ",
            @" \___ \|  \| | / _ \| ' / |  _|   ",
            @"  ___) | |\  |/ ___ \| . \ | |___  ",
            @" |____/|_| \_/_/   \_\_|\_\|_____| ",
        };

        public static IConsoleRender ShowTitle(IConsoleOutput output)
        {
            AnsiClear(output);
            int w = GetWidth();
            int h = GetHeight();

            var lines = new List<(string text, ConsoleColor color)>();
            lines.Add(("", ConsoleColor.White));
            foreach (var l in _titleArt)
                lines.Add((l, ConsoleColor.Green));
            lines.Add(("", ConsoleColor.White));
            lines.Add(("~ Terminal Snake ~", ConsoleColor.DarkGray));
            lines.Add(("", ConsoleColor.White));

            var top = ScoreManager.GetTopScores(5);
            lines.Add(("╔══════════════════════════╗", ConsoleColor.Yellow));
            lines.Add(("║        TOP 5 SCORES      ║", ConsoleColor.Yellow));
            lines.Add(("╠══════════════════════════╣", ConsoleColor.Yellow));
            if (top.Count == 0)
            {
                lines.Add(("║     No scores yet!       ║", ConsoleColor.DarkYellow));
            }
            else
            {
                for (int i = 0; i < top.Count; i++)
                    lines.Add(($"║  #{i + 1,-2}     {top[i],8} pts   ║", ConsoleColor.White));
            }
            lines.Add(("╚══════════════════════════╝", ConsoleColor.Yellow));
            lines.Add(("", ConsoleColor.White));
            lines.Add(("Choose a renderer:", ConsoleColor.Cyan));
            lines.Add(("  [1]  Standard     – classic ASCII box-drawing", ConsoleColor.White));
            lines.Add(("  [2]  Alternative  – heavy lines, neon-cyan palette", ConsoleColor.White));
            lines.Add(("  [3]  Spectre      – bold blocks, purple/magenta palette", ConsoleColor.White));

            RenderCenteredLines(lines, w, h, output);

            output.ResetColor();
            FlushInput();

            while (true)
            {
                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        return new StandardConsoleRender();
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        return new AlternativeConsoleRender();
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        return new SpectreConsoleRender();
                }
            }
        }

        public static void ShowGameOver(int score, IConsoleOutput output)
        {
            AnsiClear(output);
            int w = GetWidth();
            int h = GetHeight();

            var top = ScoreManager.GetTopScores(5);
            int rank = top.IndexOf(score) + 1;

            var lines = new List<(string text, ConsoleColor color)>();
            lines.Add(("", ConsoleColor.White));
            lines.Add(("╔══════════════════════════════════╗", ConsoleColor.Red));
            lines.Add(("║            GAME OVER!            ║", ConsoleColor.Red));
            lines.Add(("╚══════════════════════════════════╝", ConsoleColor.Red));
            lines.Add(("", ConsoleColor.White));
            lines.Add(($"Your score:  {score}  pts", ConsoleColor.White));
            if (rank > 0)
                lines.Add(($"You ranked  #{rank}  in the top 5!", ConsoleColor.Yellow));
            lines.Add(("", ConsoleColor.White));
            lines.Add((">>> Press any key to continue <<<", ConsoleColor.Cyan));

            RenderCenteredLines(lines, w, h, output);

            output.ResetColor();
            FlushInput();
            Console.ReadKey(true);
        }

        // Renders each line using ANSI absolute cursor positioning so the content
        // is vertically centred and NEVER causes the terminal to scroll, regardless
        // of how many lines there are or how tall the terminal is.
        private static void RenderCenteredLines(
            List<(string text, ConsoleColor color)> lines, int w, int h, IConsoleOutput output)
        {
            int startRow = Math.Max(0, (h - lines.Count) / 2);

            for (int i = 0; i < lines.Count; i++)
            {
                int row = startRow + i;
                if (row >= h) break;

                var (text, color) = lines[i];
                int pad = Math.Max(0, (w - text.Length) / 2);

                output.WriteAt(row + 1, 1, new string(' ', pad) + text, color);
            }
        }

        // ANSI clear-screen + cursor-home. Works in alt-screen buffers and in every
        // VT-capable terminal (VS Code, Windows Terminal, xterm, macOS Terminal).
        // Avoids the Win32 Console.Clear() APIs that interfere with conpty dimensions.
        private static void AnsiClear(IConsoleOutput output)
        {
            output.SetCursorVisible(false);
            output.Clear();
        }

        private static int GetWidth()
        {
            try { return Math.Max(60, Console.WindowWidth); }
            catch { return 80; }
        }

        private static int GetHeight()
        {
            try { return Math.Max(10, Console.WindowHeight); }
            catch { return 24; }
        }

        private static void FlushInput()
        {
            while (Console.KeyAvailable) Console.ReadKey(true);
        }
    }
}
