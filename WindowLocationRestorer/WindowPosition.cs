using System.Diagnostics;
using System.Windows;

namespace WpfApplication1
{
    public struct Rect
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
    }

    public class WindowPosition
    {
        public WindowPosition(Process process, Rect position, WindowState windowState)
        {
            Process = process;
            Position = position;
            WindowState = windowState;
        }

        public Process Process { get; set; }
        public Rect Position { get; set; }
        public WindowState WindowState { get; set; }

        public override string ToString()
        {
            return Process.MainWindowTitle;
        }
    }
}