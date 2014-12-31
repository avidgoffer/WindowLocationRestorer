using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using WindowLocationRestorer.Models;

namespace WindowLocationRestorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private IEnumerable<WindowPosition> _windowPositions;
        private List<Display> _displays;

        public MainWindow()
        {
            InitializeComponent();

            SystemEvents.SessionSwitch += SysEventsCheck;
        }

        private void SysEventsCheck(object sender, SessionSwitchEventArgs e)
        {
            switch(e.Reason)
            {
                case SessionSwitchReason.SessionLogon:
                case SessionSwitchReason.SessionUnlock:
                    RestoreWindowPositions();
                    break;
            }
        }

        private async void FormLoaded(object sender, RoutedEventArgs e)
        {
            var source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            Debug.Assert(source != null, "source != null");
            source.AddHook(WndProc);

            _windowPositions = await LoadWindowPositions();
            _displays = await LoadDisplays();

            GetDisplays();
            GetWindowPositions();
            MonitorsTextBox.Text = _displays.Aggregate("", (current, display) => current + (display.Name + " - " + display.Path + " - " + display.GdiDeviceName + "\n"));
        }

        private void GetDisplays()
        {
            foreach(var disp in InterOp.GetDisplays())
            {
                var display = _displays.FirstOrDefault(x => x.Path == disp.Path);
                if(display == null)
                {
                    _displays.Add(disp);
                }
            }
            SaveData("displays.json", _displays);
        }

        private static async Task<List<T>> LoadJson<T>(string filename)
        {
            string json;
            try
            {
                using(var reader = new StreamReader(filename))
                {
                    json = await reader.ReadToEndAsync();
                }
            }
            catch(FileNotFoundException)
            {
                json = "[]";
            }
            return JsonConvert.DeserializeObject<List<T>>(json);
        }

        private async Task<List<Display>> LoadDisplays()
        {
            return await LoadJson<Display>("displays.json");
        }

        private async Task<List<WindowPosition>> LoadWindowPositions()
        {
            return await LoadJson<WindowPosition>("windowpositions.json");
        }

        private async void SaveData(string filename, object data)
        {
            using(var writer = new StreamWriter(filename))
            {
                await writer.WriteAsync(JsonConvert.SerializeObject(data));
            }
        }

        private void SaveWindowPositionsButton_Click(object sender, RoutedEventArgs e)
        {
            GetWindowPositions();
        }

        private void GetWindowPositions()
        {
            _windowPositions = InterOp.GetWindowPositions();
            foreach(var windowPosition in _windowPositions)
            {
                var display = _displays
                    .OrderByDescending(y => y.Position.X)
                    .ThenByDescending(y => y.Position.Y)
                    .FirstOrDefault(y => (int) y.Position.X <= windowPosition.Position.X && (int) y.Position.Y <= windowPosition.Position.Y)
                              ?? _displays.First();
                windowPosition.LastDisplaySeenOn = display.Path;
            }
            SaveData("windowpositions.json", _windowPositions);
        }

        private void RestoreWindowPositionsButton_Click(object sender, RoutedEventArgs e)
        {
            RestoreWindowPositions();
        }

        private void RestoreWindowPositions()
        {
            //SaveWindowPositions();
            foreach(var display in _displays)
            {
                display.Active = false;
            }
            foreach(var disp in InterOp.GetDisplays())
            {
                var display = _displays.FirstOrDefault(x => x.Path == disp.Path);
                if(display == null)
                {
                    disp.Active = true;
                    _displays.Add(disp);
                }
                else
                {
                    display.Active = true;
                }
            }
            SaveData("displays.json", _displays);
            foreach(var windowPosition in _windowPositions)
            {
                var display = _displays.FirstOrDefault(x => x.Path == windowPosition.LastDisplaySeenOn);
                if(display == null || !display.Active) continue;
                InterOp.ShowWindow(windowPosition.Process, windowPosition.WindowState);
                InterOp.SetWindowPosition(windowPosition.Process, windowPosition.Position);
            }
        }

        private const int WM_DISPLAYCHANGE = 0x007e;

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch(msg)
            {
                case WM_DISPLAYCHANGE:
                    MonitorsTextBox.Text += "Display change detected, repositioning windows.\n";
                    Task.Run(() =>
                    {
                        RestoreWindowPositions();
                    });
                    break;
            }
            return IntPtr.Zero;
        }
    }
}