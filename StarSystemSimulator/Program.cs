using StarSystemSimulator.UI;
using System.Runtime.InteropServices;

namespace StarSystemSimulator
{
    public class Program
    {
        public static void Main()
        {
            EnableAnsiColors();

            try
            {
                var ui = new ConsoleUI();
                ui.Run();
            } catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        static void EnableAnsiColors()
        {
            if (!OperatingSystem.IsWindows()) return;

            const int STD_OUTPUT_HANDLE = -11;
            const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

            var handle = GetStdHandle(STD_OUTPUT_HANDLE);
            if (GetConsoleMode(handle, out uint mode))
            {
                SetConsoleMode(handle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
    }
}
