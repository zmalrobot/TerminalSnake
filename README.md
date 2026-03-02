# TerminalSnake (Snake)

A simple Snake-style console game written in C#/.NET.

## Prerequisites

- .NET SDK 9.x

## Build

From the repository root:

```bash
dotnet build TerminalSnake.sln -c Debug
```

## Run

```bash
dotnet run --project TerminalSnake/TerminalSnake.csproj -c Debug
```

## Controls

- Arrow keys: move the snake (Up/Down/Left/Right)

## Notes

- The game renders continuously in a tight loop (typical for console “frame” rendering).
- Background music uses `NAudio` and is Windows-only (it relies on `winmm.dll`). On Linux/macOS the game runs but music is disabled.

## Projects

- `TerminalSnake/`: the game (main loop + game logic)
- `RenderEngine/`: console render helpers + (optional) background music
