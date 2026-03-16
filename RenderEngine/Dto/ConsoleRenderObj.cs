namespace RenderEngine.Dto
{
    /// <summary>
    /// Semantic description of a single cell in the game grid.
    /// Renderers map each <see cref="CellType"/> to their own visual representation,
    /// keeping game logic completely decoupled from display details.
    /// </summary>
    public enum CellType
    {
        Empty,
        WallHorizontal,
        WallVertical,
        CornerTL,
        CornerTR,
        CornerBL,
        CornerBR,
        Head,
        Body,
        Enemy,
    }

    public class ConsoleRenderObj
    {
        public CellType Type { get; set; } = CellType.Empty;
    }
}

