using Xunit;
using TerminalSnake.Dto;
using TerminalSnake.GameLogic;
using TerminalSnake.Rendering;

namespace TerminalSnake.Tests;

/// <summary>
/// Tests for PlayerLogic.SetNextPlayerPosition (exposed as internal via InternalsVisibleTo).
/// These tests exercise the movement and collision logic without starting the game loop thread.
/// </summary>
public class PlayerMovementTests
{
    private static ConsoleConfig MakeConfig(int rows = 20, int cols = 40)
        => new ConsoleConfig { n_row = rows, n_column = cols };

    [Fact]
    public void NormalMove_Right_UpdatesHeadPosition()
    {
        var config = MakeConfig();
        var player = new PlayerDto { pos_row = 5, pos_col = 5, movmentDirection = MovmentDirection.Right };
        player.AddBody(); // one body segment needed so tail removal works

        var logic = new PlayerLogic();
        logic.SetNextPlayerPosition(config, player, 0, 1);

        Assert.Equal(5, player.pos_row);
        Assert.Equal(6, player.pos_col);
        Assert.False(player.isDead);
    }

    [Fact]
    public void NormalMove_Down_UpdatesHeadPosition()
    {
        var config = MakeConfig();
        var player = new PlayerDto { pos_row = 5, pos_col = 5 };
        player.AddBody();

        var logic = new PlayerLogic();
        logic.SetNextPlayerPosition(config, player, 1, 0);

        Assert.Equal(6, player.pos_row);
        Assert.Equal(5, player.pos_col);
        Assert.False(player.isDead);
    }

    [Theory]
    [InlineData(0,  0,  1)] // top wall  (row 0)
    [InlineData(19, 0,  1)] // bottom wall (row n_row - 1 = 19)
    [InlineData(5, -1,  0)] // left wall  (col 0)
    [InlineData(5,  0, 40)] // right wall (col n_col - 1 = 39 → next = 40)
    public void WallCollision_SetsPlayerDead(int startRow, int dR, int startCol)
    {
        // Normalise: startCol encodes the target column directly for the right-wall case.
        int row = startRow;
        int col = startCol == 40 ? 38 : startCol; // put player near the wall
        int dC  = dR == 0 ? (startCol == 40 ? 1 : -1) : 0;
        int dr  = dR;

        // For top/bottom cases adjust starting position and delta properly.
        if (dR == 1 && startRow == 0)  { row = 1; dr = -1; }           // move into row 0
        if (dR == 1 && startRow == 19) { row = 18;}                     // move into row 19

        var config = MakeConfig();
        var player = new PlayerDto { pos_row = row, pos_col = col };
        player.AddBody();

        var logic = new PlayerLogic();
        logic.SetNextPlayerPosition(config, player, dr, dC);

        Assert.True(player.isDead);
    }

    [Fact]
    public void SelfCollision_SetsDead()
    {
        var config = MakeConfig();
        // Player head at (5,5) with two body segments; the second segment is at (5,4).
        // Moving left would collide with the second segment (tail is excluded from check).
        var player = new PlayerDto { pos_row = 5, pos_col = 5 };
        player.playerBodies.Add(new PlayerBody { pos_row = 5, pos_col = 6 }); // segment 0
        player.playerBodies.Add(new PlayerBody { pos_row = 5, pos_col = 4 }); // segment 1 (tail – excluded)

        var logic = new PlayerLogic();
        // Check: checkCount = Count - 1 = 1, so only segment[0] at (5,6) is checked.
        // Moving right (dC=+1) would go to (5,6) → self-collision.
        logic.SetNextPlayerPosition(config, player, 0, 1);

        Assert.True(player.isDead);
    }

    [Fact]
    public void SelfCollision_TailExcluded_DoesNotDie()
    {
        var config = MakeConfig();
        // With exactly one body segment, checkCount = 0, so no body is checked.
        var player = new PlayerDto { pos_row = 5, pos_col = 5 };
        player.playerBodies.Add(new PlayerBody { pos_row = 5, pos_col = 6 }); // tail – excluded

        var logic = new PlayerLogic();
        logic.SetNextPlayerPosition(config, player, 0, 1); // move right to (5,6) – where tail is

        Assert.False(player.isDead); // tail was excluded so no collision
    }
}
