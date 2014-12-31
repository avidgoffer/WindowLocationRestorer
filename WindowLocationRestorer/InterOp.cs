using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using WindowLocationRestorer.Models;

namespace WindowLocationRestorer
{
    public class InterOp
    {
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, ref RECT rectangle);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;

        private const int GWL_STYLE = -16;

        private const uint WS_MINIMIZE = 0x20000000;
        private const uint WS_MAXIMIZE = 0x1000000;

        private const int SWP_ASYNCWINDOWPOS = 0x4000;

        private const int ERROR_SUCCESS = 0;

        private enum QUERY_DEVICE_CONFIG_FLAGS : uint
        {
            QDC_ALL_PATHS = 0x00000001,
            QDC_ONLY_ACTIVE_PATHS = 0x00000002,
            QDC_DATABASE_CURRENT = 0x00000004
        }

        //private enum DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY : uint
        //{
        //    DISPLAYCONFIG_OUTPUT_TECHNOLOGY_OTHER = 0xFFFFFFFF,
        //    DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HD15 = 0,
        //    DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SVIDEO = 1,
        //    DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPOSITE_VIDEO = 2,
        //    DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPONENT_VIDEO = 3,
        //    DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DVI = 4,
        //    DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HDMI = 5,
        //    DISPLAYCONFIG_OUTPUT_TECHNOLOGY_LVDS = 6,
        //    DISPLAYCONFIG_OUTPUT_TECHNOLOGY_D_JPN = 8,
        //    DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDI = 9,
        //    DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EXTERNAL = 10,
        //    DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED = 11,
        //    DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EXTERNAL = 12,
        //    DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EMBEDDED = 13,
        //    DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDTVDONGLE = 14,
        //    DISPLAYCONFIG_OUTPUT_TECHNOLOGY_MIRACAST = 15,
        //    DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL = 0x80000000,
        //    DISPLAYCONFIG_OUTPUT_TECHNOLOGY_FORCE_UINT32 = 0xFFFFFFFF
        //}

        private enum DISPLAYCONFIG_SCANLINE_ORDERING : uint
        {
            DISPLAYCONFIG_SCANLINE_ORDERING_UNSPECIFIED = 0,
            DISPLAYCONFIG_SCANLINE_ORDERING_PROGRESSIVE = 1,
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED = 2,
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_UPPERFIELDFIRST = DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED,
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_LOWERFIELDFIRST = 3,
            DISPLAYCONFIG_SCANLINE_ORDERING_FORCE_UINT32 = 0xFFFFFFFF
        }

        //private enum DISPLAYCONFIG_ROTATION : uint
        //{
        //    DISPLAYCONFIG_ROTATION_IDENTITY = 1,
        //    DISPLAYCONFIG_ROTATION_ROTATE90 = 2,
        //    DISPLAYCONFIG_ROTATION_ROTATE180 = 3,
        //    DISPLAYCONFIG_ROTATION_ROTATE270 = 4,
        //    DISPLAYCONFIG_ROTATION_FORCE_UINT32 = 0xFFFFFFFF
        //}

        //private enum DISPLAYCONFIG_SCALING : uint
        //{
        //    DISPLAYCONFIG_SCALING_IDENTITY = 1,
        //    DISPLAYCONFIG_SCALING_CENTERED = 2,
        //    DISPLAYCONFIG_SCALING_STRETCHED = 3,
        //    DISPLAYCONFIG_SCALING_ASPECTRATIOCENTEREDMAX = 4,
        //    DISPLAYCONFIG_SCALING_CUSTOM = 5,
        //    DISPLAYCONFIG_SCALING_PREFERRED = 128,
        //    DISPLAYCONFIG_SCALING_FORCE_UINT32 = 0xFFFFFFFF
        //}

        private enum DISPLAYCONFIG_PIXELFORMAT : uint
        {
            DISPLAYCONFIG_PIXELFORMAT_8BPP = 1,
            DISPLAYCONFIG_PIXELFORMAT_16BPP = 2,
            DISPLAYCONFIG_PIXELFORMAT_24BPP = 3,
            DISPLAYCONFIG_PIXELFORMAT_32BPP = 4,
            DISPLAYCONFIG_PIXELFORMAT_NONGDI = 5,
            DISPLAYCONFIG_PIXELFORMAT_FORCE_UINT32 = 0xffffffff
        }

        //private enum DISPLAYCONFIG_MODE_INFO_TYPE : uint
        //{
        //    DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE = 1,
        //    DISPLAYCONFIG_MODE_INFO_TYPE_TARGET = 2,
        //    DISPLAYCONFIG_MODE_INFO_TYPE_FORCE_UINT32 = 0xFFFFFFFF
        //}

        private enum DISPLAYCONFIG_DEVICE_INFO_TYPE : uint
        {
            DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME = 1,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME = 2,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_PREFERRED_MODE = 3,
            DISPLAYCONFIG_DEVICE_INFO_GET_ADAPTER_NAME = 4,
            DISPLAYCONFIG_DEVICE_INFO_SET_TARGET_PERSISTENCE = 5,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_BASE_TYPE = 6,
            DISPLAYCONFIG_DEVICE_INFO_FORCE_UINT32 = 0xFFFFFFFF
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LUID
        {
            public uint LowPart;
            public int HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_PATH_SOURCE_INFO
        {
            public LUID AdapterId;
            public uint Id;
            public uint ModeInfoIdx;
            public uint StatusFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_PATH_TARGET_INFO
        {
            public LUID AdapterId;
            public uint Id;
            public uint ModeInfoIdx;
            private uint OutputTechnology;
            private uint Rotation;
            private uint Scaling;
            private DISPLAYCONFIG_RATIONAL RefreshRate;
            private DISPLAYCONFIG_SCANLINE_ORDERING ScanLineOrdering;
            public bool TargetAvailable;
            public uint StatusFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_RATIONAL
        {
            public uint Numerator;
            public uint Denominator;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_PATH_INFO
        {
            public DISPLAYCONFIG_PATH_SOURCE_INFO SourceInfo;
            public DISPLAYCONFIG_PATH_TARGET_INFO TargetInfo;
            public uint Flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_2DREGION
        {
            public uint cx;
            public uint cy;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_VIDEO_SIGNAL_INFO
        {
            public ulong pixelRate;
            public DISPLAYCONFIG_RATIONAL hSyncFreq;
            public DISPLAYCONFIG_RATIONAL vSyncFreq;
            public DISPLAYCONFIG_2DREGION ActiveSize;
            public DISPLAYCONFIG_2DREGION TotalSize;
            public uint VideoStandard;
            public DISPLAYCONFIG_SCANLINE_ORDERING ScanLineOrdering;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_TARGET_MODE
        {
            public DISPLAYCONFIG_VIDEO_SIGNAL_INFO TargetVideoSignalInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINTL
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_SOURCE_MODE
        {
            public uint Width;
            public uint Height;
            public DISPLAYCONFIG_PIXELFORMAT PixelFormat;
            public POINTL Position;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct DISPLAYCONFIG_MODE_INFO_UNION
        {
            [FieldOffset(0)] public DISPLAYCONFIG_TARGET_MODE TargetMode;
            [FieldOffset(0)] public DISPLAYCONFIG_SOURCE_MODE SourceMode;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_MODE_INFO
        {
            public uint InfoType;
            public uint Id;
            public LUID AdapterId;
            public DISPLAYCONFIG_MODE_INFO_UNION ModeInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS
        {
            public uint Value;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_DEVICE_INFO_HEADER
        {
            public DISPLAYCONFIG_DEVICE_INFO_TYPE Type;
            public uint Size;
            public LUID AdapterId;
            public uint Id;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DISPLAYCONFIG_TARGET_DEVICE_NAME
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER Header;
            public DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS Flags;
            public uint OutputTechnology;
            public ushort edidManufactureId;
            public ushort edidProductCodeId;
            public uint ConnectorInstance;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public string MonitorFriendlyDeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string MonitorDevicePath;
        }

        [DllImport("user32.dll")]
        private static extern int GetDisplayConfigBufferSizes(
            QUERY_DEVICE_CONFIG_FLAGS Flags,
            out uint NumPathArrayElements,
            out uint NumModeInfoArrayElements
            );

        [DllImport("user32.dll")]
        private static extern int QueryDisplayConfig(
            QUERY_DEVICE_CONFIG_FLAGS Flags,
            ref uint NumPathArrayElements,
            [Out] DISPLAYCONFIG_PATH_INFO[] PathInfoArray,
            ref uint NumModeInfoArrayElements,
            [Out] DISPLAYCONFIG_MODE_INFO[] ModeInfoArray,
            IntPtr CurrentTopologyId
            );

        [DllImport("user32.dll")]
        private static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_SOURCE_DEVICE_NAME deviceName);

        [DllImport("user32.dll")]
        private static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_TARGET_DEVICE_NAME deviceName);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DISPLAYCONFIG_SOURCE_DEVICE_NAME
        {
            private const int Cchdevicename = 32;

            public DISPLAYCONFIG_DEVICE_INFO_HEADER Header;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Cchdevicename)] public string ViewGdiDeviceName;
        }

        private static DISPLAYCONFIG_TARGET_DEVICE_NAME GetMonitorInfo(LUID adapterId, uint targetId)
        {
            DISPLAYCONFIG_TARGET_DEVICE_NAME deviceInfo = new DISPLAYCONFIG_TARGET_DEVICE_NAME
            {
                Header =
                {
                    Size = (uint) Marshal.SizeOf(typeof(DISPLAYCONFIG_TARGET_DEVICE_NAME)),
                    AdapterId = adapterId,
                    Id = targetId,
                    Type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME
                }
            };
            int error = DisplayConfigGetDeviceInfo(ref deviceInfo);
            if(error != ERROR_SUCCESS) throw new Win32Exception(error);
            return deviceInfo;
        }

        private static DISPLAYCONFIG_SOURCE_DEVICE_NAME GetGdiDeviceNameFromSource(LUID adapterId, uint sourceId)
        {
            DISPLAYCONFIG_SOURCE_DEVICE_NAME deviceName = new DISPLAYCONFIG_SOURCE_DEVICE_NAME
            {
                Header = new DISPLAYCONFIG_DEVICE_INFO_HEADER
                {
                    Size = (uint) Marshal.SizeOf(typeof(DISPLAYCONFIG_SOURCE_DEVICE_NAME)),
                    AdapterId = adapterId,
                    Id = sourceId,
                    Type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME
                }
            };
            int error = DisplayConfigGetDeviceInfo(ref deviceName);
            if(error != ERROR_SUCCESS) throw new Win32Exception(error);
            return deviceName;
        }

        public static void ShowWindow(Process process, WindowState windowState)
        {
            switch(windowState)
            {
                case WindowState.Normal:
                    ShowWindowAsync(process.MainWindowHandle, SW_SHOWNORMAL);
                    break;
                case WindowState.Minimized:
                    ShowWindowAsync(process.MainWindowHandle, SW_SHOWMINIMIZED);
                    break;
                case WindowState.Maximized:
                    ShowWindowAsync(process.MainWindowHandle, SW_SHOWMAXIMIZED);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void SetWindowPosition(Process process, Int32Rect position)
        {
            SetWindowPos(process.MainWindowHandle, (IntPtr) 0, position.X, position.Y, position.Width, position.Height, SWP_ASYNCWINDOWPOS);
        }

        public static IEnumerable<Display> GetDisplays()
        {
            uint pathCount, modeCount;
            int error = GetDisplayConfigBufferSizes(QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS, out pathCount, out modeCount);
            if(error != ERROR_SUCCESS) throw new Win32Exception(error);

            DISPLAYCONFIG_PATH_INFO[] displayPaths = new DISPLAYCONFIG_PATH_INFO[pathCount];
            DISPLAYCONFIG_MODE_INFO[] displayModes = new DISPLAYCONFIG_MODE_INFO[modeCount];
            error = QueryDisplayConfig(QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS,
                ref pathCount, displayPaths, ref modeCount, displayModes, IntPtr.Zero);
            if(error != ERROR_SUCCESS) throw new Win32Exception(error);

            for(int i = 0; i < modeCount; i += 2)
            {
                var monitorInfo = GetMonitorInfo(displayModes[i].AdapterId, displayModes[i].Id);
                var gdiDeviceInfo = GetGdiDeviceNameFromSource(displayModes[i + 1].AdapterId, displayModes[i + 1].Id);
                yield return new Display
                {
                    Name = monitorInfo.MonitorFriendlyDeviceName,
                    Path = monitorInfo.MonitorDevicePath,
                    DisplayId = displayModes[i].Id,
                    GdiDeviceName = gdiDeviceInfo.ViewGdiDeviceName,
                    Position = new Point(displayModes[i + 1].ModeInfo.SourceMode.Position.x, displayModes[i + 1].ModeInfo.SourceMode.Position.y)
                };
            }
        }

        public static IList<WindowPosition> GetWindowPositions()
        {
            return Process.GetProcesses()
                .Select(x =>
                {
                    var rect = new RECT();
                    GetWindowRect(x.MainWindowHandle, ref rect);
                    return new {Process = x, rect};
                })
                .Where(x => x.rect.Bottom + x.rect.Left + x.rect.Right + x.rect.Top != 0)
                .Select(x =>
                {
                    var style = GetWindowLong(x.Process.MainWindowHandle, GWL_STYLE);
                    WindowState windowState;
                    if((style & WS_MAXIMIZE) == WS_MAXIMIZE)
                    {
                        windowState = WindowState.Maximized;
                    }
                    else if((style & WS_MINIMIZE) == WS_MINIMIZE)
                    {
                        windowState = WindowState.Minimized;
                    }
                    else
                    {
                        windowState = WindowState.Normal;
                    }
                    return new WindowPosition
                    {
                        Position = new Int32Rect(x.rect.Left, x.rect.Top, x.rect.Right - x.rect.Left, x.rect.Bottom - x.rect.Top),
                        ExecutableName = x.Process.Modules[0].ModuleName,
                        Process = x.Process,
                        WindowState = windowState,
                    };
                })
                .ToList();
        }
    }
}