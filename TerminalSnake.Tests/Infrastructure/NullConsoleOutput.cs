using TerminalSnake.Util;

namespace TerminalSnake.Tests.Infrastructure;

/// <summary>No-op IConsoleOutput for use in unit tests.</summary>
internal sealed class NullConsoleOutput : IConsoleOutput
{
    public void Write(string text) { }
    public void WriteLine(string text) { }
    public void Write(string text, ConsoleColor color) { }
    public void WriteLine(string text, ConsoleColor color) { }
    public void WriteAt(int row, int col, string text, ConsoleColor color) { }
    public void WriteRaw(string raw) { }
    public void Clear() { }
    public void SetCursorVisible(bool visible) { }
    public void ResetColor() { }
}
