// This example demonstrates the Console.Beep(Int32, Int32) method
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Threading;
using static RenderEngine.Dto.BackgoundMusicBase;

namespace RenderEngine
{  
    public class BackgoundMusic
    {
        private Thread? _soundThread;
        private bool _isPlaying;
       

        public BackgoundMusic()
        {
            // NAudio's WaveOutEvent uses Windows' winmm.dll. On non-Windows platforms,
            // we disable background music to keep the game runnable.
            if (!OperatingSystem.IsWindows())
            {
                return;
            }

            _soundThread = new Thread(() => Play(BackgoundMusic_Notes.Music))
            {
                Name = "Audio",
                IsBackground = true,
            };
            _soundThread.Start();
        }

        public void PlayAsync()
        {
            // Play the song
            _isPlaying = true;
        }
        public void Play()
        {           
            _isPlaying = true;
            if (OperatingSystem.IsWindows())
            {
                Play(BackgoundMusic_Notes.Music);
            }
            _isPlaying = false;
        }

        // Play the notes in a song.
        private void Play(Note[] tune)
        {
            if (!OperatingSystem.IsWindows())
            {
                return;
            }

            while (true)
            {
                if (!_isPlaying)
                {
                    Thread.Sleep(100);
                    continue;
                }

                using var wo = new WaveOutEvent();
                foreach (Note n in tune)
                {
                    if (!_isPlaying)
                    {
                        break;
                    }

                    var duration = (int)n.NoteDuration;
                    var sound = n.SignalGenerator.Take(TimeSpan.FromMilliseconds(duration));
                    wo.Init(sound);
                    wo.Play();
                    while (wo.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(50);
                    }
                }
            }
        }
    }   
}
