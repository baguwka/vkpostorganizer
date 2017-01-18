using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace vk {
   public static class AppWinApi {
      [DllImport("user32")]
      public static extern int RegisterWindowMessage(string message);

      [StringFormatMethod("format")]
      public static int RegisterWindowMessage(string format, params object[] args) {
         var message = string.Format(format, args);
         return RegisterWindowMessage(message);
      }

      public const int HWND_BROADCAST = 0xffff;
      public const int SW_SHOWNORMAL = 1;

      [DllImport("user32")]
      public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

      [DllImport("user32.dll")]
      public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

      [DllImport("user32.dll")]
      public static extern bool SetForegroundWindow(IntPtr hWnd);

      public static void ShowToFront(IntPtr window) {
         ShowWindow(window, SW_SHOWNORMAL);
         SetForegroundWindow(window);
      }
   }
}