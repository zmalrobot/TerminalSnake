using TerminalSnake.Dto;
using TerminalSnake.Rendering;
using TerminalSnake.Util;
using RenderEngine.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalSnake.GameLogic
{
    public interface IGameLogic
    {
        public string Name { get; }

        public void Initialize(ConsoleConfig consoleConfig, IConsoleOutput output, ConsoleRenderObj[,] renderMatrix, PlayerDto player, EnemyDto enemy);
        public void BeforeRender();
        public void AfterRender();
    }
}
