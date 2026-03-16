using TerminalSnake.Dto;
using TerminalSnake.Rendering;
using TerminalSnake.Util;
using RenderEngine.Dto;
using System;

namespace TerminalSnake.GameLogic
{
    public class EnemyLogic : IGameLogic
    {
        public string Name => "EnemyLogic";

        private ConsoleRenderObj[,] _renderMatrix = null!;
        private ConsoleConfig       _consoleConfig = null!;
        private PlayerDto           _player = null!;
        private EnemyDto            _enemy  = null!;
        private Random              _random = null!;

        public void Initialize(ConsoleConfig consoleConfig, IConsoleOutput output,
                               ConsoleRenderObj[,] renderMatrix, PlayerDto player, EnemyDto enemy)
        {
            _consoleConfig = consoleConfig;
            _renderMatrix  = renderMatrix;
            _player        = player;
            _enemy         = enemy;
            _random        = new Random();
        }

        public void BeforeRender()
        {
            if (!_enemy.isActive)
                SpawnNewEnemy();
            else
                _renderMatrix[_enemy.pos_row, _enemy.pos_col].Type = CellType.Enemy;
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
            if (_enemy.isActive) return;

            var randomPosR = _random.Next(1, _consoleConfig.n_row    - 2);
            var randomPosC = _random.Next(1, _consoleConfig.n_column - 2);

            if (!IsPositionOk(randomPosR, randomPosC))
            {
                SpawnNewEnemy();
                return;
            }

            // The enemy always occupies an interior (non-wall, non-player) cell,
            // so clearing the old position to Empty is always safe.
            _renderMatrix[_enemy.pos_row, _enemy.pos_col].Type = CellType.Empty;

            _enemy.pos_row  = randomPosR;
            _enemy.pos_col  = randomPosC;
            _enemy.isActive = true;

            _renderMatrix[_enemy.pos_row, _enemy.pos_col].Type = CellType.Enemy;
        }

        private bool IsPositionOk(int row, int col)
        {
            if (row == _player.pos_row && col == _player.pos_col)
                return false;

            foreach (var element in _player.playerBodies)
                if (row == element.pos_row && col == element.pos_col)
                    return false;

            return true;
        }

        private bool IsPlayerColliding()
        {
            if (_enemy == null) return false;

            if (_enemy.pos_row == _player.pos_row && _enemy.pos_col == _player.pos_col)
                return true;

            foreach (var element in _player.playerBodies)
                if (_enemy.pos_row == element.pos_row && _enemy.pos_col == element.pos_col)
                    return true;

            return false;
        }
    }
}
