using TerminalSnake.Dto;
using TerminalSnake.GameLogic;
using TerminalSnake.Rendering;
using TerminalSnake.Util;
using Pastel;
using RenderEngine;
using System;
using System.Diagnostics;
using System.Drawing;

namespace TerminalSnake
{
    public class Program
    {
        static BackgoundMusic   _backgoundMusic = null!;
        static ConsoleConfig    _consoleConfig  = null!;
        static IConsoleRender   _renderer       = null!;
        static IConsoleOutput   _titleDisplay   = null!;   // used for title/selection screens
        static IConsoleOutput   _display        = null!;   // renderer-matched, used in-game
        static IConsoleOutput   _consoleOut     = null!;   // debug output (disabled)
        static PlayerDto        _player         = null!;
        static EnemyDto         _enemy          = null!;
        static List<IGameLogic> _gameLogics     = null!;

        public static void Main(string[] args)
        {
            // Enter the alternate screen buffer (no scrollback) and hide cursor.
            // This is how vim/htop etc. prevent any terminal scrolling.
            // Supported by VS Code, Windows Terminal, xterm, and all VT hosts.
            bool useAltScreen = !Console.IsOutputRedirected;
            if (useAltScreen)
                Console.Write("\x1B[?1049h\x1B[?25l");

            try
            {
                InitOnce();

                while (true)
                {
                    _renderer = TitleScreen.ShowTitle(_titleDisplay);
                    // Create an output implementation that matches the chosen renderer.
                    _display  = _renderer is SpectreConsoleRender
                                    ? (IConsoleOutput)new ConsoleOutSpectre()
                                    : new ConsoleOut(_consoleConfig, true);
                    InitGame();
                    RunGame();
                    ScoreManager.SaveScore(_player.points);
                    TitleScreen.ShowGameOver(_player.points, _display);
                }
            }
            catch (Exception ex)
            {
                // Restore normal screen before printing the error.
                if (useAltScreen)
                    Console.Write("\x1B[?1049l\x1B[?25h");
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (useAltScreen)
                    Console.Write("\x1B[?1049l\x1B[?25h");
            }
        }

        // One-time startup: console config, debug output, and background music.
        private static void InitOnce()
        {
            _consoleConfig = new ConsoleConfig();
            _consoleOut    = new ConsoleOut(_consoleConfig, false);
            _titleDisplay  = new ConsoleOut(_consoleConfig, true);

            _backgoundMusic = new BackgoundMusic();
            _backgoundMusic.Play();
        }

        // Per-game initialisation: render matrix, player, enemy, and all game logics.
        private static void InitGame()
        {
            // Re-read terminal size at the start of every game so the grid adapts
            // if the window was resized between rounds (or before the first round).
            _consoleConfig = new ConsoleConfig();
            _consoleOut    = new ConsoleOut(_consoleConfig, false);

            _renderer.Initialize(_consoleConfig, _display);

            _player    = new PlayerDto();
            _enemy     = new EnemyDto();
            _gameLogics = new List<IGameLogic>();

            foreach (Type gameLogic in System.Reflection.Assembly.GetExecutingAssembly()
                         .GetTypes()
                         .Where(t => t.GetInterfaces().Contains(typeof(IGameLogic))))
            {
                var instance = (IGameLogic?)Activator.CreateInstance(gameLogic);
                if (instance != null)
                {
                    instance.Initialize(_consoleConfig, _consoleOut,
                                        _renderer.GetRenderMatrix(), _player, _enemy);
                    _gameLogics.Add(instance);
                }
            }

            _renderer.Clean();
        }

        // Main game loop – runs until the player dies.
        private static void RunGame()
        {
            var debugData = new List<string>();
            var headers   = new List<string>();

            double frameMs   = 1000.0 / _consoleConfig.target_fps;
            double nextFrameAt = 0;
            var sw = Stopwatch.StartNew();

            do
            {
                headers = new List<string>()
                {
                    $"Point: {_player.points}"
                };

                _gameLogics.ForEach(x => x.BeforeRender());

/*#if DEBUG
                debugData = new List<string>()
                {
                    $"FieldSize: Row {_consoleConfig.n_row} - Col {_consoleConfig.n_column}",
                    $"FieldSize Usable:",
                    $"          Row: 1 - {_consoleConfig.n_row - 2}",
                    $"          Col: 1 - {_consoleConfig.n_column - 2}",
                    $"Player Position: {_player.pos_row} - {_player.pos_col} \t Enemy Position: {_enemy.pos_row} - {_enemy.pos_col}"
                };
#endif*/

                _renderer.Render(headers, debugData);
                _gameLogics.ForEach(x => x.AfterRender());

                // Sleep for the remaining time until the next frame deadline.
                // This replaces the busy-wait and keeps CPU usage near zero.
                nextFrameAt += frameMs;
                int sleepMs = (int)(nextFrameAt - sw.Elapsed.TotalMilliseconds);
                if (sleepMs > 0)
                    Thread.Sleep(sleepMs);
                else
                    nextFrameAt = sw.Elapsed.TotalMilliseconds; // catch up if behind

            } while (!_player.isDead);
        }
    }
}