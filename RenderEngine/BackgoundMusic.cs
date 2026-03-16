// Background music player – Windows uses NAudio/WaveOut, Linux/macOS use aplay/afplay
// with in-memory WAV generation so no extra NuGet packages are required.
using NAudio.Wave;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using static RenderEngine.Dto.BackgoundMusicBase;

namespace RenderEngine
{
    public class BackgoundMusic
    {
        private Thread?          _soundThread;
        private volatile bool    _isPlaying;
        // WAV is generated once at startup – all platforms share the same bytes.
        private readonly byte[]  _wavBytes;

        public BackgoundMusic()
        {
            _wavBytes = GenerateWav(BackgoundMusic_Notes.Music);

            if (OperatingSystem.IsWindows())
            {
                _soundThread = new Thread(PlayWindowsLoop)
                {
                    Name         = "Audio",
                    IsBackground = true,
                };
            }
            else if (OperatingSystem.IsLinux())
            {
                _soundThread = new Thread(() => PlayExternalLoop("aplay", "-q"))
                {
                    Name         = "Audio",
                    IsBackground = true,
                };
            }
            else if (OperatingSystem.IsMacOS())
            {
                _soundThread = new Thread(() => PlayExternalLoop("afplay", ""))
                {
                    Name         = "Audio",
                    IsBackground = true,
                };
            }

            _soundThread?.Start();
        }

        /// <summary>Signal the background thread to start playing.</summary>
        public void Play()
        {
            _isPlaying = true;
        }

        /// <summary>Same as <see cref="Play"/>; kept for API compatibility.</summary>
        public void PlayAsync()
        {
            _isPlaying = true;
        }

        // ── Windows: play the pre-built WAV as one continuous buffer via NAudio ─
        // Previously this played note-by-note with Thread.Sleep(50) polling between
        // each note, adding up to 50 ms of silence per note – stretching the tempo.
        // Now the entire tune is sent to WaveOutEvent as a single stream; playback
        // finishes via an event, with zero gaps between notes.
        private void PlayWindowsLoop()
        {
            while (true)
            {
                if (!_isPlaying) { Thread.Sleep(100); continue; }

                using var ms     = new MemoryStream(_wavBytes);
                using var reader = new WaveFileReader(ms);
                using var wo     = new WaveOutEvent();
                var done = new ManualResetEventSlim(false);
                wo.PlaybackStopped += (_, _) => done.Set();
                wo.Init(reader);
                wo.Play();
                done.Wait();
            }
        }

        // ── Linux / macOS: write pre-built WAV to a temp file and play via aplay ─
        private void PlayExternalLoop(string player, string args)
        {
            while (true)
            {
                if (!_isPlaying) { Thread.Sleep(100); continue; }

                var tmpFile = Path.Combine(Path.GetTempPath(),
                                           $"terminalsnake_{Guid.NewGuid()}.wav");
                try
                {
                    File.WriteAllBytes(tmpFile, _wavBytes);

                    var psi = new ProcessStartInfo(player,
                                                   string.IsNullOrEmpty(args)
                                                       ? $"\"{tmpFile}\""
                                                       : $"{args} \"{tmpFile}\"")
                    {
                        RedirectStandardError = true,
                        UseShellExecute       = false,
                    };

                    using var proc = Process.Start(psi);
                    proc?.WaitForExit();
                }
                catch
                {
                    // player binary not found or audio subsystem unavailable – keep trying
                    Thread.Sleep(500);
                }
                finally
                {
                    try { if (File.Exists(tmpFile)) File.Delete(tmpFile); } catch { }
                }
            }
        }

        // ── PCM WAV generator (no extra dependencies) ────────────────────────────
        private static byte[] GenerateWav(Note[] tune)
        {
            const int   sampleRate    = 44100;
            const short bitsPerSample = 16;
            const short channels      = 1;

            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            // Reserve space for the header; we fill the data size later.
            bw.Write(new byte[44]);

            int sampleCount = 0;
            foreach (var note in tune)
            {
                int freq       = (int)note.NoteTone;
                int numSamples = sampleRate * (int)note.NoteDuration / 1000;

                for (int i = 0; i < numSamples; i++)
                {
                    short sample = 0;
                    if (freq > 0)
                    {
                        double t = (double)i / sampleRate;
                        sample = (short)(Math.Sin(2 * Math.PI * freq * t) * 0.2 * short.MaxValue);
                    }
                    bw.Write(sample);
                }
                sampleCount += numSamples;
            }

            int dataSize = sampleCount * sizeof(short);

            // Write RIFF/WAVE header at position 0.
            ms.Position = 0;
            bw.Write(new byte[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F' });
            bw.Write(36 + dataSize);                                    // ChunkSize
            bw.Write(new byte[] { (byte)'W', (byte)'A', (byte)'V', (byte)'E' });
            bw.Write(new byte[] { (byte)'f', (byte)'m', (byte)'t', (byte)' ' });
            bw.Write(16);                                               // Subchunk1Size (PCM)
            bw.Write((short)1);                                         // AudioFormat = PCM
            bw.Write(channels);
            bw.Write(sampleRate);
            bw.Write(sampleRate * channels * bitsPerSample / 8);        // ByteRate
            bw.Write((short)(channels * bitsPerSample / 8));            // BlockAlign
            bw.Write(bitsPerSample);
            bw.Write(new byte[] { (byte)'d', (byte)'a', (byte)'t', (byte)'a' });
            bw.Write(dataSize);

            return ms.ToArray();
        }
    }
}

