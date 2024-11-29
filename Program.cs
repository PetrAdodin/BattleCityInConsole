using System;
using System.Threading;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetCurrentConsoleFontEx(IntPtr consoleOutput, bool maximumWindow, ref CONSOLE_FONT_INFO_EX consoleCurrentFontEx);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int handle);

    private const int STD_OUTPUT_HANDLE = -11;

    [StructLayout(LayoutKind.Sequential)]
    private struct COORD
    {
        public short X;
        public short Y;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct CONSOLE_FONT_INFO_EX
    {
        public uint cbSize;
        public uint nFont;
        public COORD dwFontSize;
        public int FontFamily;
        public int FontWeight;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string FaceName;
    }

    // Метод для настройки консоли перед игрой.
    static void SetUpConsole()
    {
        Console.CursorVisible = false; // Скрытие курсора.
        Console.Title = "Tank Game"; // Заголовок окна консоли.

        IntPtr hnd = GetStdHandle(STD_OUTPUT_HANDLE);
        CONSOLE_FONT_INFO_EX fontInfo = new CONSOLE_FONT_INFO_EX();
        fontInfo.cbSize = (uint)Marshal.SizeOf(fontInfo);

        // Устанавливаем желаемый размер шрифта
        fontInfo.dwFontSize = new COORD { X = 54, Y = 54 }; // Увеличиваем Y для увеличения размера шрифта

        // Сохраняем текущий шрифт
        fontInfo.FaceName = "Lucida Console"; // Текущий шрифт или оставить пустым для сохранения
        SetCurrentConsoleFontEx(hnd, false, ref fontInfo);

        int _consoleWidth = 13;
        int _consoleHeight = 13;
        Console.SetWindowSize(_consoleWidth, _consoleHeight);
        Console.SetBufferSize(_consoleWidth, _consoleHeight);

    }

    static void Main()
    {
        SetUpConsole();
        Console.WriteLine("Esc - leave\nN - new game\nE - continue");

        var temp = true;
        while (temp)
        {
            if (Console.KeyAvailable)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.N:
                        Game game = new Game();
                        game.Run();
                        break;
                    case ConsoleKey.Escape:
                        temp = false;
                        break;
                    case ConsoleKey.E:
                        Game game1 = new Game();
                        game1 = game1.LoadGame();
                        game1.Run();
                        break;
                }
            }
        }


    }
}