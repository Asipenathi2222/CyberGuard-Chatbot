using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

// AudioPlayer lives ONLY in this file.
// The duplicate that was inside MemoryStore.cs has been removed.
namespace CybersecurityChatbot.Utilities
{
    /// <summary>
    /// Plays greeting.wav on startup using the Windows PlaySound API.
    /// Uses SND_ASYNC so playback runs in the background and the WPF
    /// window opens immediately without any freeze or delay.
    /// </summary>
    public static class AudioPlayer
    {
        // SND_ASYNC  = play on a background thread (never blocks UI)
        // SND_FILENAME = pszSound is a file path
        // SND_NODEFAULT = stay silent if the file is missing
        private const uint SND_ASYNC = 0x0001;
        private const uint SND_FILENAME = 0x00020000;
        private const uint SND_NODEFAULT = 0x00000002;

        [DllImport("winmm.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool PlaySound(string pszSound, IntPtr hmod, uint fdwSound);

        private static bool _played = false;

        /// <summary>
        /// Plays greeting.wav once. Subsequent calls are ignored.
        /// Wraps playback in Task.Run so it never touches the UI thread.
        /// </summary>
        public static void PlayGreeting()
        {
            if (_played) return;
            _played = true;

            Task.Run(() =>
            {
                try
                {
                    string audioPath = Path.Combine(AppContext.BaseDirectory, "greeting.wav");

                    if (!File.Exists(audioPath))
                    {
                        Debug.WriteLine($"[AudioPlayer] greeting.wav not found at: {audioPath}");
                        return;
                    }

                    if (OperatingSystem.IsWindows())
                    {
                        // SND_ASYNC lets winmm.dll handle threading internally
                        PlaySound(audioPath, IntPtr.Zero,
                                  SND_FILENAME | SND_ASYNC | SND_NODEFAULT);
                    }
                    else if (OperatingSystem.IsMacOS())
                    {
                        RunProcess("afplay", $"\"{audioPath}\"");
                    }
                    else if (OperatingSystem.IsLinux())
                    {
                        if (CommandExists("paplay")) RunProcess("paplay", $"\"{audioPath}\"");
                        else if (CommandExists("aplay")) RunProcess("aplay", $"\"{audioPath}\"");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[AudioPlayer] Exception: {ex.Message}");
                }
            });
        }

        // ── Helpers ───────────────────────────────────────────────────

        private static void RunProcess(string exe, string args)
        {
            var psi = new ProcessStartInfo
            {
                FileName = exe,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var p = Process.Start(psi);
            p?.WaitForExit();
        }

        private static bool CommandExists(string name)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "which",
                    Arguments = name,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var p = Process.Start(psi);
                if (p == null) return false;
                p.WaitForExit();
                return p.ExitCode == 0;
            }
            catch { return false; }
        }
    }
}