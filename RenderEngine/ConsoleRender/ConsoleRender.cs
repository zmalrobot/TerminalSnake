using Pastel;
using RenderEngine.Dto;
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
        private long _lastRenderMillis;
        private long _lastFpsMillis;
        private long _timeDeltaMillis;
        private long _timeCounter;

        private ConsoleConfig _consoleConfig;
        private ConsoleRenderObj[,] _renderMatrix;
        public ConsoleRender(ConsoleConfig consoleConfig) 
        {
            _stopwatch = new Stopwatch(); 
            _consoleConfig = consoleConfig;
            Console.CursorVisible = false;
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
            _timeDeltaMillis = 1000 / _consoleConfig.target_fps;

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

            for (int row = 0; row < _consoleConfig.n_row; row++)
            {
                _renderMatrix[row, 0] = new ConsoleRenderObj()
                {
                    character = '#',
                    color = ConsoleColors.White
                };

                _renderMatrix[row, _consoleConfig.n_column - 1] = new ConsoleRenderObj()
                {
                    character = '#',
                    color = ConsoleColors.White
                };
            }

            for (int col = 0; col < _consoleConfig.n_column; col++)
            {
                _renderMatrix[0, col] = new ConsoleRenderObj()
                {
                    character = '#',
                    color = ConsoleColors.White
                };
                _renderMatrix[_consoleConfig.n_row - 1, col] = new ConsoleRenderObj()
                {
                    character = '#',
                    color = ConsoleColors.White
                };
            }

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
            Console.Clear();
        }
        public void Render(List<string> headers, List<string> additionalData = null) 
        {
       
            if (!CanRender())
                return;

            _timeCounter++;

            var stringBuilder = new StringBuilder();

            if (headers != null)
                foreach (var element in headers)
                {
                    stringBuilder.AppendLine(element);
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

            stringBuilder.Append(ComputeFps());
            stringBuilder.Append(Environment.NewLine);
            
            if(additionalData != null)
                foreach (var element in additionalData)
                {
                    stringBuilder.AppendLine(element);
                }

            
            //Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.Write(stringBuilder.ToString());            
        }
       
        private string ComputeFps() {

            var currentmillis = _stopwatch.ElapsedMilliseconds;
            var diveder = (currentmillis - _lastFpsMillis);

            if(diveder == 0)
                return ($"FPS: 0");

            var fps = (float) _timeCounter / diveder;
            fps = fps * 1000; //Ragiono in millisecondi
            _lastFpsMillis = currentmillis;
            _timeCounter = 0;
            return ($"FPS: {fps.ToString("#.0")}");
        }
        private bool CanRender()
        {
            var currentmillis = _stopwatch.ElapsedMilliseconds;

            if ((currentmillis - _lastRenderMillis) > _timeDeltaMillis)
            {
                _lastRenderMillis = currentmillis;
                return true;
            }                

            return false;
        }
    }
}
