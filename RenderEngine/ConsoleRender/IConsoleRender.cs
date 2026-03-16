using RenderEngine.Dto;
using TerminalSnake.Util;
using System.Collections.Generic;

namespace TerminalSnake.Rendering
{
    /// <summary>
    /// Contract for all game renderers.
    /// Implement this interface to add a new rendering backend
    /// (different tile set, GUI library, remote display, etc.).
    /// </summary>
    public interface IConsoleRender
    {
        /// <summary>Human-readable name shown in the renderer selection menu.</summary>
        string Name { get; }

        /// <summary>
        /// Initialise the renderer for a new game session.
        /// Builds the semantic render matrix, sets up timing, and writes the
        /// initial blank frame.  Must be called before any <see cref="Render"/> call.
        /// </summary>
        void Initialize(ConsoleConfig config, IConsoleOutput output);

        /// <summary>Returns the shared semantic render matrix written by game logic.</summary>
        ConsoleRenderObj[,] GetRenderMatrix();

        /// <summary>Erase the entire screen (cursor-home).</summary>
        void Clean();

        /// <summary>
        /// Render one frame.  The call is rate-limited internally; frames that
        /// arrive before the configured target-FPS deadline are silently dropped.
        /// </summary>
        void Render(List<string> headers, List<string>? additionalData = null);
    }
}
