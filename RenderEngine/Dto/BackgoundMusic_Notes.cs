using static RenderEngine.Dto.BackgoundMusicBase;

namespace RenderEngine
{
    public static class BackgoundMusic_Notes
    {
        // "Snake Chase" – 80s NES / C64 chiptune.
        // Phrase A is a Tetris-style hook; B varies it; C is a fast arpeggio burst
        // (pure C64 feel); D closes with a triumphant scale run before the loop.
        public static Note[] Music =
        {
            // ── Phrase A: main melodic hook (Tetris-inspired) ───────────────────
            new Note(Tone.E,        Duration.QUARTER),
            new Note(Tone.B,        Duration.EIGHTH),
            new Note(Tone.C,        Duration.EIGHTH),
            new Note(Tone.D,        Duration.QUARTER),
            new Note(Tone.C,        Duration.EIGHTH),
            new Note(Tone.B,        Duration.EIGHTH),
            new Note(Tone.A,        Duration.QUARTER),
            new Note(Tone.A,        Duration.EIGHTH),
            new Note(Tone.C,        Duration.EIGHTH),
            new Note(Tone.E,        Duration.QUARTER),
            new Note(Tone.D,        Duration.EIGHTH),
            new Note(Tone.C,        Duration.EIGHTH),
            new Note(Tone.B,        Duration.QUARTER),
            new Note(Tone.B,        Duration.EIGHTH),
            new Note(Tone.C,        Duration.EIGHTH),
            new Note(Tone.D,        Duration.QUARTER),
            new Note(Tone.E,        Duration.QUARTER),
            new Note(Tone.C,        Duration.QUARTER),
            new Note(Tone.A,        Duration.QUARTER),
            new Note(Tone.A,        Duration.HALF),

            // ── Phrase B: contrasting voice ─────────────────────────────────
            new Note(Tone.D,        Duration.QUARTER),
            new Note(Tone.D,        Duration.EIGHTH),
            new Note(Tone.F,        Duration.EIGHTH),
            new Note(Tone.A,        Duration.QUARTER),
            new Note(Tone.Gsharp,   Duration.EIGHTH),
            new Note(Tone.F,        Duration.EIGHTH),
            new Note(Tone.Gsharp,   Duration.QUARTER),
            new Note(Tone.Gsharp,   Duration.EIGHTH),
            new Note(Tone.A,        Duration.EIGHTH),
            new Note(Tone.B,        Duration.QUARTER),
            new Note(Tone.E,        Duration.EIGHTH),
            new Note(Tone.E,        Duration.EIGHTH),
            new Note(Tone.C,        Duration.EIGHTH),
            new Note(Tone.A,        Duration.EIGHTH),
            new Note(Tone.A,        Duration.HALF),

            // ── Phrase C: fast sixteenth-note arpeggio burst (C64 style) ────────
            new Note(Tone.A,        Duration.SIXTEENTH),
            new Note(Tone.C,        Duration.SIXTEENTH),
            new Note(Tone.E,        Duration.SIXTEENTH),
            new Note(Tone.A,        Duration.SIXTEENTH),
            new Note(Tone.Gsharp,   Duration.SIXTEENTH),
            new Note(Tone.E,        Duration.SIXTEENTH),
            new Note(Tone.Gsharp,   Duration.SIXTEENTH),
            new Note(Tone.B,        Duration.SIXTEENTH),
            new Note(Tone.F,        Duration.SIXTEENTH),
            new Note(Tone.D,        Duration.SIXTEENTH),
            new Note(Tone.F,        Duration.SIXTEENTH),
            new Note(Tone.A,        Duration.SIXTEENTH),
            new Note(Tone.E,        Duration.SIXTEENTH),
            new Note(Tone.C,        Duration.SIXTEENTH),
            new Note(Tone.A,        Duration.SIXTEENTH),
            new Note(Tone.GbelowC,  Duration.SIXTEENTH),

            // ── Phrase D: ascending scale charge + resolution ──────────────────
            new Note(Tone.GbelowC,  Duration.EIGHTH),
            new Note(Tone.A,        Duration.EIGHTH),
            new Note(Tone.B,        Duration.EIGHTH),
            new Note(Tone.C,        Duration.EIGHTH),
            new Note(Tone.D,        Duration.EIGHTH),
            new Note(Tone.E,        Duration.EIGHTH),
            new Note(Tone.Fsharp,   Duration.EIGHTH),
            new Note(Tone.G,        Duration.QUARTER),
            new Note(Tone.E,        Duration.EIGHTH),
            new Note(Tone.C,        Duration.EIGHTH),
            new Note(Tone.A,        Duration.HALF),
            new Note(Tone.REST,     Duration.QUARTER),
        };
    }
}
