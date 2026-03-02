# TerminalSnake

Snake-style console game written in C#/.NET.

It uses a simple “frame” renderer (a matrix of characters + colors) and updates the console by moving the cursor back to `(0,0)` every frame.

## Requirements

- .NET SDK 9.x
- A terminal that supports ANSI escape sequences (for colored output)
- A font that can display the game characters (`■`, `≡`, `░`) — most modern monospace fonts work

## Quick start

From the repository root:

```bash
dotnet restore
dotnet build TerminalSnake.sln -c Debug
dotnet run --project TerminalSnake/TerminalSnake.csproj -c Debug
```

## Controls

- Arrow keys: change direction (Up / Down / Left / Right)

Input is read on a background thread and the game throttles how frequently it accepts a new direction.

## How the game works

### Game loop

The main loop lives in `TerminalSnake/Program.cs`:

- Creates a `ConsoleConfig` (field size + target FPS)
- Initializes the render matrix (`ConsoleRender.InitalizeRender()`)
- Discovers and instantiates all `IGameLogic` implementations via reflection
- Repeats forever:
	- `BeforeRender()` on each logic
	- render the frame
	- `AfterRender()` on each logic

In Debug builds, extra diagnostic lines are rendered (field size and entity positions).

### Rendering

Rendering is based on a 2D matrix of `ConsoleRenderObj` cells:

- Each cell holds a character and a logical color (`ConsoleColors`)
- Colors are mapped to hex strings and applied via the `Pastel` library
- The renderer tries to run at `target_fps` by skipping renders until the next frame time

### Gameplay

- The snake moves inside a bordered playfield (`#` characters)
- Picking up the enemy (food) increases score and grows the snake by one segment
- Food spawns at a random empty position (not on the snake)

## Configuration

The simplest way to tweak the game is to edit `ConsoleConfig` in `RenderEngine/ConsoleRender/ConsoleConfig.cs`:

- `n_column` (default: `100`) — playfield width
- `n_row` (default: `20`) — playfield height
- `target_fps` (default: `10`) — target update/render rate

Notes:

- Your terminal window needs to be big enough to display the whole frame (plus a few header/debug lines).
- If the output looks “smeared” or scrolls, it usually means the terminal is too small or cursor positioning is not supported.

## Build & run

### Debug

```bash
dotnet build TerminalSnake.sln -c Debug
dotnet run --project TerminalSnake/TerminalSnake.csproj -c Debug
```

### Release

```bash
dotnet build TerminalSnake.sln -c Release
dotnet run --project TerminalSnake/TerminalSnake.csproj -c Release
```

## Platform notes (Linux/macOS)

- The project references `NAudio` for background music.
- `NAudio` output uses Windows APIs (`winmm.dll`) via `WaveOutEvent`.
- On non-Windows platforms, the game disables background music at runtime (so the game remains playable).

If you see audio-related runtime errors on a non-Windows OS, open an issue with your OS + terminal details.

## Project structure

- `TerminalSnake/`
	- `Program.cs`: entrypoint + main loop
	- `GameLogic/`: gameplay systems
		- `IGameLogic.cs`: plugin interface (`Initialize`, `BeforeRender`, `AfterRender`)
		- `PlayerLogic.cs`: movement + input
		- `EnemyLogic.cs`: food spawning + collision
	- `Dto/`: player/enemy state containers
- `RenderEngine/`
	- `ConsoleRender/`: rendering utilities (`ConsoleConfig`, `ConsoleRender`)
	- `Dto/`: render/music DTOs
	- `BackgoundMusic.cs`: optional background music (Windows only)

## Troubleshooting

- **No colors / weird characters**: ensure your terminal supports ANSI color and your font supports `■`, `≡`, `░`.
- **Screen scrolls instead of updating in place**: resize your terminal larger, and make sure it supports cursor positioning.
- **Very slow / high CPU**: lower `target_fps` in `ConsoleConfig`.

