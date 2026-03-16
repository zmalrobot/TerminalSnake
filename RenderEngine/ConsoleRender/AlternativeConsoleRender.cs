using Pastel;
using RenderEngine.Dto;
using System.Collections.Generic;

namespace TerminalSnake.Rendering
{
    /// <summary>
    /// Alternative tileset with heavy box-drawing walls and a neon-cyan palette.
    /// Uses Pastel for 24-bit ANSI colour output (Linux + Windows compatible).
    /// </summary>
    public class AlternativeConsoleRender : ConsoleRenderBase
    {
        public override string Name => "Alternative";

        private static readonly Dictionary<CellType, (char Ch, string Hex)> Tiles = new()
        {
            [CellType.Empty]          = (' ', "#000000"),
            [CellType.WallHorizontal] = ('━', "#00FFFF"),
            [CellType.WallVertical]   = ('┃', "#00FFFF"),
            [CellType.CornerTL]       = ('┏', "#00FFFF"),
            [CellType.CornerTR]       = ('┓', "#00FFFF"),
            [CellType.CornerBL]       = ('┗', "#00FFFF"),
            [CellType.CornerBR]       = ('┛', "#00FFFF"),
            [CellType.Head]           = ('@', "#00FFFF"),
            [CellType.Body]           = ('▪', "#0088FF"),
            [CellType.Enemy]          = ('★', "#FF8800"),
        };

        protected override string RenderCell(CellType type)
        {
            var (ch, hex) = Tiles[type];
            return ch.ToString().Pastel(hex);
        }
    }
}
