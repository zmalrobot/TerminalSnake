using System.Text.Json;

namespace TerminalSnake.Util
{
    public static class ScoreManager
    {
        private static readonly string _savePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TerminalSnake", "scores.json");

        public static void SaveScore(int score)
        {
            var scores = LoadAll();
            scores.Add(score);
            scores.Sort((a, b) => b.CompareTo(a));
            var dir = Path.GetDirectoryName(_savePath)!;
            Directory.CreateDirectory(dir);
            File.WriteAllText(_savePath, JsonSerializer.Serialize(scores));
        }

        public static List<int> GetTopScores(int count = 10)
        {
            var scores = LoadAll();
            scores.Sort((a, b) => b.CompareTo(a));
            return scores.Take(count).ToList();
        }

        private static List<int> LoadAll()
        {
            if (!File.Exists(_savePath))
                return new List<int>();
            try
            {
                var json = File.ReadAllText(_savePath);
                return JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
            }
            catch
            {
                return new List<int>();
            }
        }
    }
}
