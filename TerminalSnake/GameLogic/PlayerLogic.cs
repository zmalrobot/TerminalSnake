using TerminalSnake.Dto;
using TerminalSnake.Rendering;
using TerminalSnake.Util;
using Newtonsoft.Json;
using RenderEngine.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TerminalSnake.GameLogic
{
    public class PlayerLogic : IGameLogic
    {
        private Stopwatch _stopwatch;
        private long _lastMovmentMills;
        private long _timeDeltaMillis = 1000;
        public string Name => "PlayerLogic";
        private ConsoleRenderObj[,] _renderMatrix;
        private ConsoleConfig _consoleConfig;
        private ConsoleOut _consoleOut;
        private PlayerDto _prev;
        private PlayerDto _current;
        private EnemyDto _enemy;
        private Thread _readKey;

        private bool canAceptInput = true;

        public void Initialize(ConsoleConfig consoleConfig, ConsoleOut consoleOut, ConsoleRenderObj[,] renderMatrix, PlayerDto player, EnemyDto enemy)
        {
            _consoleConfig = consoleConfig;
            _consoleOut = consoleOut;
            _renderMatrix = renderMatrix;
            _current = player;
            _enemy = enemy;

            _timeDeltaMillis = 1000 / _consoleConfig.target_fps;

            _readKey = new Thread(() =>
            {
                while (true)
                {
                    while (Console.KeyAvailable && canAceptInput)
                    {
                        if (canAceptInput)
                        {
                            ConsoleKeyInfo key = Console.ReadKey(true);
                            switch (key.Key)
                            {
                                case ConsoleKey.RightArrow:
                                    player.movmentDirection = MovmentDirection.Right;
                                    break;
                                case ConsoleKey.LeftArrow:
                                    player.movmentDirection = MovmentDirection.Left;
                                    break;
                                case ConsoleKey.UpArrow:
                                    player.movmentDirection = MovmentDirection.Up;
                                    break;
                                case ConsoleKey.DownArrow:
                                    player.movmentDirection = MovmentDirection.Down;
                                    break;
                                default:
                                    break;
                            }
                        }                       

                        canAceptInput = false;                       
                    }

                    Thread.Sleep(200);
                    canAceptInput = true;
                }
            });
            _readKey.Start();

            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public void BeforeRender()
        {           
            _prev = JsonConvert.DeserializeObject<PlayerDto>(JsonConvert.SerializeObject(_current));

            var currentmillis = _stopwatch.ElapsedMilliseconds;

            if ((currentmillis - _lastMovmentMills) > _timeDeltaMillis)
            {
                _lastMovmentMills = currentmillis;

                switch (_current.movmentDirection)
                {
                    case MovmentDirection.Up:
                        SetNextPlayerPosition(_consoleConfig, _current, -1, 0);
                        break;
                    case MovmentDirection.Down:
                        SetNextPlayerPosition(_consoleConfig, _current, 1, 0);
                        break;
                    case MovmentDirection.Right:
                        SetNextPlayerPosition(_consoleConfig, _current, 0, 1);
                        break;
                    case MovmentDirection.Left:
                        SetNextPlayerPosition(_consoleConfig, _current, 0, -1);
                        break;

                }
            }
        }

        public void AfterRender()
        {
            UpdateBodyPosition();
            UpdatePlayerPosition();
        }

        private void SetNextPlayerPosition(ConsoleConfig consoleConfig, PlayerDto player, int incR = 0, int incC = 0)
        {
            var nextR = player.pos_row + incR;
            var nextC = player.pos_col + incC;

            if (nextR < 1)
                return;

            if (nextR > (consoleConfig.n_row - 2))
                return;

            if (nextC <  1)
                return;

            if (nextC > (consoleConfig.n_column - 2))
                return;

            //Mi porto con me tutto il corpo
            player.playerBodies.Insert(0, new PlayerBody()
            {
                pos_col = player.pos_col,
                pos_row = player.pos_row,
            });
            player.playerBodies.RemoveAt(player.playerBodies.Count() - 1);

            player.pos_row = nextR;
            player.pos_col = nextC;
        }

        private void UpdateBodyPosition()
        {
            foreach (var body in _prev.playerBodies)
            {
                _renderMatrix[body.pos_row, body.pos_col].color = ConsoleColors.Black;
                _renderMatrix[body.pos_row, body.pos_col].character = ' ';
            }

            foreach (var body in _current.playerBodies)
            {
                _renderMatrix[body.pos_row, body.pos_col].color = body.body_color;
                _renderMatrix[body.pos_row, body.pos_col].character = body.body_char;
            }

        }
        private void UpdatePlayerPosition()
        {
            _renderMatrix[_prev.pos_row, _prev.pos_col].color = ConsoleColors.Black;
            _renderMatrix[_prev.pos_row, _prev.pos_col].character = ' ';

            _renderMatrix[_current.pos_row, _current.pos_col].color = _current.player_color;
            _renderMatrix[_current.pos_row, _current.pos_col].character = _current.player_char;
        }

      


    }
}
