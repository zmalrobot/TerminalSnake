# TerminalSnake

A Snake-style game that runs in any ANSI-capable terminal, written in C# / .NET 9.

## Requirements

- .NET SDK 9.x
- A terminal that supports ANSI escape sequences and 24-bit colour (VS Code, Windows Terminal, xterm, any Linux pty)
- A font with full Unicode box-drawing and block characters

## Quick start

```bash
dotnet restore
dotnet build TerminalSnake.sln -c Debug
dotnet run --project TerminalSnake/TerminalSnake.csproj -c Debug
```

## Controls

| Key | Action |
|---|---|
| Arrow keys | Change direction |
| `1` | Select Standard renderer |
| `2` | Select Alternative renderer |
| `3` | Select Spectre renderer |

## How it works

### Game loop

`Program.cs` drives everything:

1. Shows the title / renderer-selection screen
2. Instantiates the chosen renderer and matching `IConsoleOutput` implementation
3. Discovers and creates all `IGameLogic` plugins via reflection
4. Runs the game loop: `BeforeRender()` → `Render()` → `AfterRender()`
5. After the player dies, saves the score and shows the game-over screen

The loop sleeps for the remaining time until the next frame deadline, so CPU usage stays near zero regardless of FPS target.

### Rendering system

The render pipeline is fully abstracted behind two interfaces.

#### `IConsoleRender`

All renderers implement this interface. Adding a new renderer means:

1. Subclass `ConsoleRenderBase`
2. Implement `RenderCell(CellType)` with your chosen glyphs and colours
3. Add an entry to the selection menu in `TitleScreen.ShowTitle`

Three renderers are included:

| # | Class | Tileset | Palette |
|---|---|---|---|
| 1 | `StandardConsoleRender` | `╔═║■≡░` | White walls · Green snake · Red food |
| 2 | `AlternativeConsoleRender` | `┏━┃@▪★` | Cyan walls · Blue snake · Orange food |
| 3 | `SpectreConsoleRender` | `▛▬█◆◇⊕` | Purple walls · Magenta snake · Orange-red food |

#### `IConsoleOutput`

All terminal I/O goes through this interface. Two implementations are provided:

| Class | Backend | Used for |
|---|---|---|
| `ConsoleOut` | Pastel (ANSI 24-bit colour) | Standard and Alternative renderers |
| `ConsoleOutSpectre` | Spectre.Console | Spectre renderer – title/game-over screens get rich styled output; the high-frequency game loop still uses raw ANSI for performance |

#### Semantic render matrix (`CellType`)

Game logic writes only semantic `CellType` values into the shared `ConsoleRenderObj[,]` matrix.
Renderers read that matrix and decide which glyph and colour to use — game logic contains zero visual information.

```
CellType: Empty | WallHorizontal | WallVertical | CornerTL | CornerTR | CornerBL | CornerBR
          Head  | Body           | Enemy
```

### Timing

- `ConsoleRenderBase` rate-limits frames to `target_fps` using accumulator-style timing (no drift).
- `PlayerLogic` uses the same accumulator pattern for movement ticks.
- The main loop in `Program.cs` sleeps for the remaining frame budget instead of busy-waiting.
- The FPS counter is a 500 ms sliding window for a stable reading.

### Background music

Music is generated as a WAV in memory at startup and looped on a dedicated background thread, completely decoupled from the game loop:

- **Windows** – played via NAudio `WaveOutEvent`; completion is event-driven (no polling gaps between notes).
- **Linux** – piped to `aplay -q` via a temp file.
- **macOS** – piped to `afplay`.

If the audio binary is not found the thread silently retries every 500 ms; the game remains fully playable.

## Configuration

Edit `ConsoleConfig` in `RenderEngine/ConsoleRender/ConsoleConfig.cs`:

| Property | Default | Description |
|---|---|---|
| `target_fps` | `10` | Frame and movement tick rate |
| `n_column` | auto (window width − 1) | Playfield width |
| `n_row` | auto (window height − overhead) | Playfield height |

The grid auto-sizes to the terminal window at the start of each game round.

## Build

```bash
# Debug
dotnet build TerminalSnake.sln -c Debug

# Release
dotnet build TerminalSnake.sln -c Release
```

## CI

A Gitea Actions workflow (`.gitea/workflows/build.yml`) builds the solution on every push to `main` using the `mcr.microsoft.com/dotnet/sdk:9.0` container on a `linux64-latest` runner.

## Project structure

```
TerminalSnake.sln
├── TerminalSnake/                      Game executable
│   ├── Program.cs                      Entry point + main loop
│   ├── TitleScreen.cs                  Title, renderer selection, game-over screen
│   ├── GameLogic/
│   │   ├── IGameLogic.cs               Plugin interface (Initialize / BeforeRender / AfterRender)
│   │   ├── PlayerLogic.cs              Movement, input, self-collision, wall-collision
│   │   └── EnemyLogic.cs               Food spawning + player collision detection
│   ├── Dto/
│   │   ├── PlayerDto.cs                Player + body state (position only, no visuals)
│   │   └── EnemyDto.cs                 Food state (position only, no visuals)
│   └── Util/
│       └── ScoreManager.cs             Persist and load high scores
└── RenderEngine/                       Class library (shared by all renderers)
    ├── ConsoleRender/
    │   ├── IConsoleRender.cs           Renderer contract
    │   ├── ConsoleRenderBase.cs        Shared matrix init, wall setup, FPS timing, Render loop
    │   ├── StandardConsoleRender.cs    Classic box-drawing tileset
    │   ├── AlternativeConsoleRender.cs Heavy-line neon tileset
    │   ├── SpectreConsoleRender.cs     Block / Spectre-inspired tileset
    │   └── ConsoleConfig.cs            Grid size + FPS configuration
    ├── Dto/
    │   ├── ConsoleRenderObj.cs         Grid cell holding a CellType
    │   └── BackgoundMusic_Notes.cs     Note sequence for the background tune
    ├── Util/
    │   ├── IConsoleOutput.cs           Terminal output abstraction
    │   ├── ConsoleOut.cs               Pastel / ANSI implementation
    │   └── ConsoleOutSpectre.cs        Spectre.Console implementation
    └── BackgoundMusic.cs               Cross-platform music loop (Windows/Linux/macOS)
```

## Troubleshooting

| Symptom | Fix |
|---|---|
| No colour / boxes show as `?` | Use a terminal with ANSI 24-bit colour and a Unicode-capable font |
| Screen scrolls instead of updating in place | Make the terminal window taller / wider |
| High CPU usage | Lower `target_fps` in `ConsoleConfig` |
| No sound on Linux | Install `aplay` (`apt install alsa-utils`) |
| No sound on macOS | `afplay` is built-in; check system audio permissions |
