using Pastel;
using RenderEngine.Dto;
using System.Collections.Generic;

namespace TerminalSnake.Rendering
{
    /// <summary>
    /// Classic ASCII/box-drawing tileset – the original look of Terminal Snake.
    /// Uses Pastel for 24-bit ANSI colour output (Linux + Windows compatible).
    /// </summary>
    public class StandardConsoleRender : ConsoleRenderBase
    {
        public override string Name => "Standard";

        private static readonly Dictionary<CellType, (char Ch, string Hex)> Tiles = new()
        {
            [CellType.Empty]          = (' ', "#000000"),
            [CellType.WallHorizontal] = ('═', "#FFFFFF"),
            [CellType.WallVertical]   = ('║', "#FFFFFF"),
            [CellType.CornerTL]       = ('╔', "#FFFFFF"),
            [CellType.CornerTR]       = ('╗', "#FFFFFF"),
            [CellType.CornerBL]       = ('╚', "#FFFFFF"),
            [CellType.CornerBR]       = ('╝', "#FFFFFF"),
            [CellType.Head]           = ('■', "#00FF00"),
            [CellType.Body]           = ('≡', "#00FF00"),
            [CellType.Enemy]          = ('░', "#FF0000"),
        };

        protected override string RenderCell(CellType type)
        {
            var (ch, hex) = Tiles[type];
            return ch.ToString().Pastel(hex);
        }
    }
}
