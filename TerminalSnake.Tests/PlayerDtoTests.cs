using Xunit;
using TerminalSnake.Dto;

namespace TerminalSnake.Tests;

public class PlayerDtoTests
{
    [Fact]
    public void AddScore_IncrementsPoints()
    {
        var player = new PlayerDto();
        player.AddScore(10);
        Assert.Equal(10, player.points);
    }

    [Fact]
    public void AddScore_AccumulatesAcrossMultipleCalls()
    {
        var player = new PlayerDto();
        player.AddScore(10);
        player.AddScore(25);
        Assert.Equal(35, player.points);
    }

    [Fact]
    public void AddBody_AddsBodySegmentAtCurrentHeadPosition()
    {
        var player = new PlayerDto { pos_row = 7, pos_col = 3 };
        player.AddBody();

        Assert.Single(player.playerBodies);
        Assert.Equal(7, player.playerBodies[0].pos_row);
        Assert.Equal(3, player.playerBodies[0].pos_col);
    }

    [Fact]
    public void AddBody_EachCallAppendsASegment()
    {
        var player = new PlayerDto { pos_row = 1, pos_col = 1 };
        player.AddBody();
        player.AddBody();
        Assert.Equal(2, player.playerBodies.Count);
    }

    [Fact]
    public void InitialState_IsAlive_WithZeroPoints()
    {
        var player = new PlayerDto();
        Assert.False(player.isDead);
        Assert.Equal(0, player.points);
        Assert.Empty(player.playerBodies);
    }
}
