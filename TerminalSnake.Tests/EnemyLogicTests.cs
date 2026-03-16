using Xunit;
using RenderEngine.Dto;
using TerminalSnake.Dto;
using TerminalSnake.GameLogic;
using TerminalSnake.Rendering;
using TerminalSnake.Tests.Infrastructure;

namespace TerminalSnake.Tests;

public class EnemyLogicTests
{
    // Creates a fully populated render matrix for the given config.
    private static ConsoleRenderObj[,] CreateMatrix(ConsoleConfig config)
    {
        var matrix = new ConsoleRenderObj[config.n_row, config.n_column];
        for (int r = 0; r < config.n_row; r++)
            for (int c = 0; c < config.n_column; c++)
                matrix[r, c] = new ConsoleRenderObj();
        return matrix;
    }

    [Fact]
    public void BeforeRender_WhenEnemyInactive_SpawnsAndActivatesEnemy()
    {
        var config = new ConsoleConfig { n_row = 20, n_column = 40 };
        var matrix = CreateMatrix(config);
        var player = new PlayerDto { pos_row = 2, pos_col = 2 };
        var enemy  = new EnemyDto  { isActive = false };

        var logic = new EnemyLogic();
        logic.Initialize(config, new NullConsoleOutput(), matrix, player, enemy);
        logic.BeforeRender();

        Assert.True(enemy.isActive);
    }

    [Fact]
    public void BeforeRender_WhenEnemyActive_SetsEnemyCellInMatrix()
    {
        var config = new ConsoleConfig { n_row = 20, n_column = 40 };
        var matrix = CreateMatrix(config);
        var player = new PlayerDto { pos_row = 2, pos_col = 2 };
        var enemy  = new EnemyDto  { isActive = true, pos_row = 5, pos_col = 5 };

        var logic = new EnemyLogic();
        logic.Initialize(config, new NullConsoleOutput(), matrix, player, enemy);
        logic.BeforeRender();

        Assert.Equal(CellType.Enemy, matrix[5, 5].Type);
    }

    [Fact]
    public void AfterRender_WhenPlayerHeadOnEnemy_PlayerScoresAndGrowsBody()
    {
        var config = new ConsoleConfig { n_row = 20, n_column = 40 };
        var matrix = CreateMatrix(config);
        // Player standing exactly where the enemy is.
        var player = new PlayerDto { pos_row = 5, pos_col = 5 };
        var enemy  = new EnemyDto  { isActive = true, pos_row = 5, pos_col = 5, point = 10 };

        var logic = new EnemyLogic();
        logic.Initialize(config, new NullConsoleOutput(), matrix, player, enemy);
        logic.AfterRender();

        Assert.Equal(10, player.points);
        Assert.Single(player.playerBodies);
    }

    [Fact]
    public void AfterRender_WhenPlayerBodyOnEnemy_PlayerScoresAndGrowsBody()
    {
        var config = new ConsoleConfig { n_row = 20, n_column = 40 };
        var matrix = CreateMatrix(config);
        var player = new PlayerDto { pos_row = 3, pos_col = 3 };
        player.playerBodies.Add(new PlayerBody { pos_row = 5, pos_col = 5 });
        var enemy = new EnemyDto { isActive = true, pos_row = 5, pos_col = 5, point = 10 };

        var logic = new EnemyLogic();
        logic.Initialize(config, new NullConsoleOutput(), matrix, player, enemy);
        logic.AfterRender();

        Assert.Equal(10, player.points);
        Assert.Equal(2, player.playerBodies.Count); // original body + new growth
    }

    [Fact]
    public void AfterRender_WhenNoCollision_ScoreUnchanged()
    {
        var config = new ConsoleConfig { n_row = 20, n_column = 40 };
        var matrix = CreateMatrix(config);
        var player = new PlayerDto { pos_row = 2, pos_col = 2 };
        var enemy  = new EnemyDto  { isActive = true, pos_row = 8, pos_col = 8, point = 10 };

        var logic = new EnemyLogic();
        logic.Initialize(config, new NullConsoleOutput(), matrix, player, enemy);
        logic.AfterRender();

        Assert.Equal(0, player.points);
        Assert.Empty(player.playerBodies);
    }
}
