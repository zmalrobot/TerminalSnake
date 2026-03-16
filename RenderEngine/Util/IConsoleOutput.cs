using System;

namespace TerminalSnake.Util
{
    /// <summary>
    /// Abstraction over all terminal output operations.
    /// Implement this interface to swap in alternative output targets
    /// (e.g. ncurses backend, GUI overlay, file recorder, or test mock).
    /// </summary>
    public interface IConsoleOutput
    {
        /// <summary>Write text at the current cursor position.</summary>
        void Write(string text);

        /// <summary>Write text followed by a newline.</summary>
        void WriteLine(string text);

        /// <summary>Write colored text at the current cursor position.</summary>
        void Write(string text, ConsoleColor color);

        /// <summary>Write colored text followed by a newline.</summary>
        void WriteLine(string text, ConsoleColor color);

        /// <summary>
        /// Write text at the specified absolute terminal position (1-based row and column).
        /// Uses ANSI cursor positioning: ESC[row;colH.
        /// </summary>
        void WriteAt(int row, int col, string text, ConsoleColor color);

        /// <summary>
        /// Write a pre-formatted string verbatim.
        /// The string may contain embedded ANSI escape sequences (e.g. Pastel color codes).
        /// Intended for full-frame batch output in the render engine.
        /// </summary>
        void WriteRaw(string raw);

        /// <summary>Clear the entire terminal screen and move the cursor to the home position.</summary>
        void Clear();

        /// <summary>Show or hide the terminal cursor.</summary>
        void SetCursorVisible(bool visible);

        /// <summary>Reset all terminal color and formatting attributes.</summary>
        void ResetColor();
    }
}
