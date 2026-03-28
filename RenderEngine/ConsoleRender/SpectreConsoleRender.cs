using Pastel;
using RenderEngine.Dto;
using System.Collections.Generic;

namespace TerminalSnake.Rendering
{
    /// <summary>
    /// Spectre-inspired tileset: bold block walls (purple) and vibrant game pieces.
    /// Uses <see cref="ConsoleOutSpectre"/> for rich UI (title/game-over screens) while
    /// the high-frequency game loop still uses Pastel raw ANSI for performance.
    /// </summary>
    public class SpectreConsoleRender : ConsoleRenderBase
    {
        public override string Name => "Spectre";

        private static readonly Dictionary<CellType, (char Ch, string Hex)> Tiles = new()
        {
            [CellType.Empty]          = (' ', "#000000"),
            [CellType.WallHorizontal] = ('▬', "#AF00FF"),
            [CellType.WallVertical]   = ('█', "#AF00FF"),
            [CellType.CornerTL]       = ('▛', "#AF00FF"),
            [CellType.CornerTR]       = ('▜', "#AF00FF"),
            [CellType.CornerBL]       = ('▙', "#AF00FF"),
            [CellType.CornerBR]       = ('▟', "#AF00FF"),
            [CellType.Head]           = ('◆', "#FF00FF"),
            [CellType.Body]           = ('◇', "#AF00FF"),
            [CellType.Enemy]          = ('⊕', "#FF4500"),
        };

        protected override string RenderCell(CellType type)
        {
            var (ch, hex) = Tiles[type];
            return ch.ToString().Pastel(hex);
        }
    }
}
