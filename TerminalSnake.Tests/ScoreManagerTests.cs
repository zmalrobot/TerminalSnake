using Xunit;
using TerminalSnake.Util;

namespace TerminalSnake.Tests;

/// <summary>
/// Tests for ScoreManager. Uses IDisposable to back up and restore any existing
/// scores file so local developer state is never corrupted.
/// </summary>
public class ScoreManagerTests : IDisposable
{
    private readonly string _savePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "TerminalSnake", "scores.json");

    private readonly string? _backup;

    public ScoreManagerTests()
    {
        if (File.Exists(_savePath))
        {
            _backup = File.ReadAllText(_savePath);
            File.Delete(_savePath);
        }
    }

    public void Dispose()
    {
        if (_backup != null)
            File.WriteAllText(_savePath, _backup);
        else if (File.Exists(_savePath))
            File.Delete(_savePath);
    }

    [Fact]
    public void GetTopScores_WhenNoFile_ReturnsEmptyList()
    {
        var scores = ScoreManager.GetTopScores();
        Assert.Empty(scores);
    }

    [Fact]
    public void SaveScore_ThenGetTopScores_ReturnsScore()
    {
        ScoreManager.SaveScore(42);
        var scores = ScoreManager.GetTopScores();

        Assert.Single(scores);
        Assert.Equal(42, scores[0]);
    }

    [Fact]
    public void SaveScore_MultipleScores_ReturnedSortedDescending()
    {
        ScoreManager.SaveScore(10);
        ScoreManager.SaveScore(50);
        ScoreManager.SaveScore(30);

        var scores = ScoreManager.GetTopScores();

        Assert.Equal(new[] { 50, 30, 10 }, scores);
    }

    [Fact]
    public void GetTopScores_RespectsCountLimit()
    {
        for (int i = 1; i <= 15; i++)
            ScoreManager.SaveScore(i * 10);

        var top5 = ScoreManager.GetTopScores(count: 5);

        Assert.Equal(5, top5.Count);
        Assert.Equal(150, top5[0]); // highest first
    }
}
