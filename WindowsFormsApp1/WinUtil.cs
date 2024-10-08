﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class WinUtil
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr WindowHandle);

        [DllImport("user32.dll")]
        public static extern long LockWindowUpdate(IntPtr Handle);

        private const int SW_MAXIMIZE = 3;
        private const int SW_MINIMIZE = 6;
        public const int SW_RESTORE = 9;
        [DllImport("dibalcrypt.dll")]
        public static extern int Decrypt(Int32[] sBufferIn, int longInput, ref byte bufferOut, ref int ptrLongOutput, Int32 myE, Int32 myD, Int32 myN);
        [DllImport("dibalcrypt.dll")]
        public static extern int Encrypt(string sBufferIn, ref byte sBufferOut, ref int ptrLongOutput, Int32 destE, Int32 destN);
        [DllImport("dibalcrypt.dll")]
        public static extern int Decrypt(byte[] sBufferIn, int longInput, ref byte bufferOut, ref int ptrLongOutput, Int32 myE, Int32 myD, Int32 myN);
        [DllImport("dibalcrypt.dll")]
        public static extern void GenerateKeys(ref Int32 myE, ref Int32 myN, ref Int32 myD);

        public static void FocusProcess(string procName)
        {
            Process[] objProcesses = System.Diagnostics.Process.GetProcessesByName(procName);
            if (objProcesses.Length > 0)
            {
                IntPtr hWnd = IntPtr.Zero;
                hWnd = objProcesses[0].MainWindowHandle;
                ShowWindowAsync(new HandleRef(null, hWnd), SW_MAXIMIZE);
                SetForegroundWindow(objProcesses[0].MainWindowHandle);
            }
        }

        public static int GetWindowCount(int processId, out List<IntPtr> lh)
        {
            IntPtr hShellWindow = GetShellWindow();
            int count = 0;
            List<IntPtr> l = new List<IntPtr>();
            EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                if (hWnd == hShellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                uint windowPid;
                GetWindowThreadProcessId(hWnd, out windowPid);
                if (windowPid != processId) return true;

                count++;
                l.Add(hWnd);


                return true;
            }, 0);

            lh = l;

            return count;
        }

        public static string GetWindowTitle(IntPtr hWnd)
        {
            int textLength = GetWindowTextLength(hWnd);
            StringBuilder outText = new StringBuilder(textLength + 1);
            int a = GetWindowText(hWnd, outText, outText.Capacity);
            return outText.ToString();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        private const UInt32 WM_CLOSE = 0x0010;

        public static void CloseWindow(IntPtr hwnd)
        {
            SendMessage(hwnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }
        public const int cryp_E = 5969;
        public const int cryp_N = 3472429;
        public const int decryp_D = 1050977;
    }
}
