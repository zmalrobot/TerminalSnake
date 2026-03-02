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
    public class EnemyLogic : IGameLogic
    {
        public string Name => "EnemyLogic";
        private ConsoleRenderObj _renderMatrixPrev;
        private ConsoleRenderObj[,] _renderMatrix;
        private ConsoleConfig _consoleConfig;
        private ConsoleOut _consoleOut;
        private PlayerDto _player;
        private EnemyDto _enemy;
        private PlayerDto _enemyPlayer;

        private Random _random;

        public void Initialize(ConsoleConfig consoleConfig, ConsoleOut consoleOut, ConsoleRenderObj[,] renderMatrix, PlayerDto player, EnemyDto enemy)
        {
            _consoleConfig = consoleConfig;
            _consoleOut = consoleOut;
            _renderMatrix = renderMatrix;
            _player = player;
            _enemy = enemy;
            _random = new Random();

            _renderMatrixPrev = JsonConvert.DeserializeObject<ConsoleRenderObj>(JsonConvert.SerializeObject(_renderMatrix[1, 1]));
        }

        public void BeforeRender()
        {
            if(!_enemy.isActive)
            {
                SpawnNewEnemy(); //Iizio del gioco
            }
            else
            {
                _renderMatrix[_enemy.pos_row, _enemy.pos_col].color = _enemy.enemy_color;
                _renderMatrix[_enemy.pos_row, _enemy.pos_col].character = _enemy.enemy_char;
            }            
        }

        public void AfterRender()
        {
            if (IsPlayerColliding())
            {
                _player.AddScore(_enemy.point);
                _player.AddBody();

                _enemy.isActive = false;
                SpawnNewEnemy();
            }
        }

        private void SpawnNewEnemy()
        {
            if (!_enemy.isActive)
            {
                var randomPosR = _random.Next(1, _consoleConfig.n_row - 2);
                var randomPosC = _random.Next(1, _consoleConfig.n_column - 2);

                if(!IsPositionOk(randomPosR, randomPosC))
                {
                    SpawnNewEnemy();
                    return;
                }

                
                _renderMatrix[_enemy.pos_row, _enemy.pos_col].color = _renderMatrixPrev.color;
                _renderMatrix[_enemy.pos_row, _enemy.pos_col].character = _renderMatrixPrev.character;
                               

                _renderMatrixPrev = JsonConvert.DeserializeObject<ConsoleRenderObj>(JsonConvert.SerializeObject(_renderMatrix[randomPosR, randomPosC]));
                _enemy.pos_row = randomPosR;
                _enemy.pos_col = randomPosC;
                _enemy.isActive = true;
            }
        }

        private bool IsPositionOk(int row, int col)
        {            
            if (row == _player.pos_row && col == _player.pos_col)
                return false;

            foreach(var element in _player.playerBodies)
            {
                if (row == element.pos_row && col == element.pos_col)
                    return false;
            }

            return true;
        }

        private bool IsPlayerColliding()
        {
            if(_enemy == null)
                return false;

            if (_enemy.pos_row == _player.pos_row && _enemy.pos_col == _player.pos_col)
                return true;

            foreach (var element in _player.playerBodies)
            {
                if (_enemy.pos_row == element.pos_row && _enemy.pos_col == element.pos_col)
                    return true;
            }

            return false;
        }

    }
}
