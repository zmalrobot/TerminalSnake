using Pastel;
using RenderEngine.Dto;
using TerminalSnake.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TerminalSnake.Rendering
{
    /// <summary>
    /// Abstract base for all console renderers.
    /// Provides the shared semantic render matrix, FPS-limited Render loop,
    /// timing, and wall initialisation.  Subclasses only need to implement
    /// <see cref="RenderCell"/> to map each <see cref="CellType"/> to a
    /// display string (typically an ANSI-coloured character via Pastel).
    /// </summary>
    public abstract class ConsoleRenderBase : IConsoleRender
    {
        private Stopwatch _stopwatch = new();
        private double _lastRenderMillis;
        private long   _lastFpsMillis;
        private double _timeDeltaMillis;
        private long   _timeCounter;
        private double _fps;

        protected ConsoleConfig      _consoleConfig = null!;
        protected IConsoleOutput     _output        = null!;
        protected ConsoleRenderObj[,] _renderMatrix  = null!;

        public abstract string Name { get; }

        public ConsoleRenderObj[,] GetRenderMatrix() => _renderMatrix;

        // ── Initialisation ───────────────────────────────────────────────────────

        public void Initialize(ConsoleConfig config, IConsoleOutput output)
        {
            _consoleConfig       = config;
            _output              = output;
            _timeDeltaMillis     = 1000.0 / config.target_fps;
            _lastRenderMillis    = 0;
            _fps                 = 0;
            _timeCounter         = 0;
            _output.SetCursorVisible(false);

            // Build the semantic matrix.
            _renderMatrix = new ConsoleRenderObj[config.n_row, config.n_column];
            for (int r = 0; r < config.n_row; r++)
                for (int c = 0; c < config.n_column; c++)
                    _renderMatrix[r, c] = new ConsoleRenderObj();

            // Walls (semantic – each renderer decides the glyph & colour).
            for (int row = 1; row < config.n_row - 1; row++)
            {
                _renderMatrix[row, 0].Type                   = CellType.WallVertical;
                _renderMatrix[row, config.n_column - 1].Type = CellType.WallVertical;
            }
            for (int col = 1; col < config.n_column - 1; col++)
            {
                _renderMatrix[0, col].Type                   = CellType.WallHorizontal;
                _renderMatrix[config.n_row - 1, col].Type    = CellType.WallHorizontal;
            }
            _renderMatrix[0, 0].Type                                            = CellType.CornerTL;
            _renderMatrix[0, config.n_column - 1].Type                          = CellType.CornerTR;
            _renderMatrix[config.n_row - 1, 0].Type                             = CellType.CornerBL;
            _renderMatrix[config.n_row - 1, config.n_column - 1].Type           = CellType.CornerBR;

            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            Clean();
        }

        public void Clean() => _output.Clear();

        // ── Rendering ────────────────────────────────────────────────────────────

        /// <summary>
        /// Map a single <see cref="CellType"/> to the string that will be written
        /// to the terminal (typically a Pastel-coloured single character).
        /// </summary>
        protected abstract string RenderCell(CellType type);

        public void Render(List<string> headers, List<string>? additionalData = null)
        {
            if (!CanRender()) return;

            _timeCounter++;

            var sb = new StringBuilder();

            // Cursor-home is the first thing in the buffer so the entire frame
            // output is one atomic Console.Write – no flicker gap.
            sb.Append("\x1B[H");

            if (headers != null)
                foreach (var h in headers)
                {
                    var line = h ?? string.Empty;
                    // Pad to full width so stale characters from the previous
                    // frame are overwritten with spaces.
                    if (line.Length < _consoleConfig.n_column)
                        line += new string(' ', _consoleConfig.n_column - line.Length);
                    sb.AppendLine(line);
                }

            for (int row = 0; row < _consoleConfig.n_row; row++)
            {
                for (int col = 0; col < _consoleConfig.n_column; col++)
                    sb.Append(RenderCell(_renderMatrix[row, col].Type));
                sb.Append(Environment.NewLine);
            }

            if (additionalData != null)
                foreach (var d in additionalData)
                    sb.AppendLine(d);

            // Overlay FPS counter at the far right of the header row.
            var fpsText = ComputeFps();
            int fpsCol  = Math.Max(1, _consoleConfig.n_column - fpsText.Length + 1);
            sb.Append($"\x1B[1;{fpsCol}H{fpsText}");

            // Strip trailing newlines – cursor must not scroll past the last row.
            while (sb.Length > 0 &&
                   (sb[sb.Length - 1] == '\n' || sb[sb.Length - 1] == '\r'))
                sb.Length--;

            _output.WriteRaw(sb.ToString());
        }

        // ── Private timing helpers ────────────────────────────────────────────────

        private string ComputeFps()
        {
            var current = _stopwatch.ElapsedMilliseconds;
            var elapsed = current - _lastFpsMillis;
            // Refresh the displayed value every 500 ms for a stable reading.
            if (elapsed >= 500)
            {
                _fps          = elapsed > 0 ? _timeCounter * 1000.0 / elapsed : 0;
                _lastFpsMillis = current;
                _timeCounter   = 0;
            }
            return $"FPS: {_fps.ToString("#.0")}";
        }

        private bool CanRender()
        {
            var current = _stopwatch.ElapsedMilliseconds;
            if (current - _lastRenderMillis >= _timeDeltaMillis)
            {
                _lastRenderMillis += _timeDeltaMillis;
                // Resync if we've fallen more than one frame behind.
                if (current - _lastRenderMillis > _timeDeltaMillis)
                    _lastRenderMillis = current;
                return true;
            }
            return false;
        }
    }
}
