using Pastel;
using RenderEngine.Dto;
using TerminalSnake.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalSnake.Rendering
{
    public class ConsoleRender
    {
        private Stopwatch _stopwatch;
        private double _lastRenderMillis;
        private long _lastFpsMillis;
        private double _timeDeltaMillis;
        private long _timeCounter;
        private double _fps;

        private ConsoleConfig _consoleConfig;
        private ConsoleRenderObj[,] _renderMatrix;
        private IConsoleOutput _output;

        public ConsoleRender(ConsoleConfig consoleConfig, IConsoleOutput output)
        {
            _stopwatch = new Stopwatch();
            _consoleConfig = consoleConfig;
            _output = output;
            _output.SetCursorVisible(false);
        }
         
        public ConsoleRenderObj[,] GetRenderMatrix()
        {
            return _renderMatrix;
        }

        public void InitalizeRender()
        {
            _stopwatch.Start();
            _timeCounter = 0;
            _lastRenderMillis = 0;
            _fps = 0;
            _timeDeltaMillis = 1000.0 / _consoleConfig.target_fps;

            _renderMatrix = new ConsoleRenderObj[_consoleConfig.n_row, _consoleConfig.n_column];

            for (int col = 0; col < _consoleConfig.n_column; col++)
            {
                for (int row = 0; row < _consoleConfig.n_row; row++)
                {
                    _renderMatrix[row, col] = new ConsoleRenderObj()
                    {
                        character = ' ',
                        color = ConsoleColors.Black
                    };
                }
            }

            // Left and right walls: ║  (corners handled below)
            for (int row = 1; row < _consoleConfig.n_row - 1; row++)
            {
                _renderMatrix[row, 0] = new ConsoleRenderObj()
                {
                    character = '║',
                    color = ConsoleColors.White
                };

                _renderMatrix[row, _consoleConfig.n_column - 1] = new ConsoleRenderObj()
                {
                    character = '║',
                    color = ConsoleColors.White
                };
            }

            // Top and bottom walls: ═  (corners handled below)
            for (int col = 1; col < _consoleConfig.n_column - 1; col++)
            {
                _renderMatrix[0, col] = new ConsoleRenderObj()
                {
                    character = '═',
                    color = ConsoleColors.White
                };
                _renderMatrix[_consoleConfig.n_row - 1, col] = new ConsoleRenderObj()
                {
                    character = '═',
                    color = ConsoleColors.White
                };
            }

            // Corners
            _renderMatrix[0, 0]                                                    = new ConsoleRenderObj() { character = '╔', color = ConsoleColors.White };
            _renderMatrix[0, _consoleConfig.n_column - 1]                          = new ConsoleRenderObj() { character = '╗', color = ConsoleColors.White };
            _renderMatrix[_consoleConfig.n_row - 1, 0]                             = new ConsoleRenderObj() { character = '╚', color = ConsoleColors.White };
            _renderMatrix[_consoleConfig.n_row - 1, _consoleConfig.n_column - 1]   = new ConsoleRenderObj() { character = '╝', color = ConsoleColors.White };

            Clean();
            //Render();
        }
        public void SetCell(int row, int col, char character, ConsoleColors color)
        {
            _renderMatrix[row, col].character = character;
            _renderMatrix[row, col].color = color;
        }
        public void Clean()
        {
            _output.Clear();
        }
        public void Render(List<string> headers, List<string> additionalData = null)
        {
            if (!CanRender())
                return;

            _timeCounter++;

            var stringBuilder = new StringBuilder();

            // \x1B[H = cursor-home. Embedding it as the FIRST bytes of the single
            // Console.Write() makes repositioning and frame output one atomic operation.
            // A separate SetCursorPosition() call has a gap where a stray newline can
            // scroll the terminal and cause the up-down bounce flicker.
            stringBuilder.Append("\x1B[H");

            if (headers != null)
                foreach (var element in headers)
                {
                    // Pad every header to the full column width so any stale coloured
                    // characters from the previous frame are overwritten with spaces.
                    var line = element ?? string.Empty;
                    if (line.Length < _consoleConfig.n_column)
                        line += new string(' ', _consoleConfig.n_column - line.Length);
                    stringBuilder.AppendLine(line);
                }

            for (int row = 0; row < _consoleConfig.n_row; row++)
            {
                for (int col = 0; col < _consoleConfig.n_column; col++)
                {
                    var element = _renderMatrix[row, col];
                    var color = _consoleConfig.color_mapping[element.color];
                    var elementColored = $"{element.character}".Pastel(color);
                    stringBuilder.Append(elementColored);
                }
                stringBuilder.Append(Environment.NewLine);
            }

            if (additionalData != null)
                foreach (var element in additionalData)
                    stringBuilder.AppendLine(element);

            // Overlay FPS at the top-right corner using ANSI absolute positioning.
            // Row 1 is the header line (score); we write on the far right of that row.
            var fpsText = ComputeFps();
            int fpsCol = Math.Max(1, _consoleConfig.n_column - fpsText.Length + 1);
            stringBuilder.Append($"\x1B[1;{fpsCol}H{fpsText}");

            // Strip every trailing CR/LF. The cursor must NOT advance past the last
            // visible row, because doing so scrolls the terminal up by one line.
            while (stringBuilder.Length > 0 &&
                   (stringBuilder[stringBuilder.Length - 1] == '\n' ||
                    stringBuilder[stringBuilder.Length - 1] == '\r'))
            {
                stringBuilder.Length--;
            }

            _output.WriteRaw(stringBuilder.ToString());
        }
       
        private string ComputeFps()
        {
            var currentmillis = _stopwatch.ElapsedMilliseconds;
            var elapsed = currentmillis - _lastFpsMillis;

            // Refresh the displayed FPS value every 500 ms for a stable reading.
            if (elapsed >= 500)
            {
                _fps = elapsed > 0 ? _timeCounter * 1000.0 / elapsed : 0;
                _lastFpsMillis = currentmillis;
                _timeCounter = 0;
            }

            return $"FPS: {_fps.ToString("#.0")}";
        }
        private bool CanRender()
        {
            var currentmillis = _stopwatch.ElapsedMilliseconds;

            if ((currentmillis - _lastRenderMillis) >= _timeDeltaMillis)
            {
                _lastRenderMillis += _timeDeltaMillis;
                // If we've fallen more than one frame behind, resync to now.
                if (currentmillis - _lastRenderMillis > _timeDeltaMillis)
                    _lastRenderMillis = currentmillis;
                return true;
            }

            return false;
        }
    }
}
