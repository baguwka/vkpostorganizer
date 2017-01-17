using System;
using System.Diagnostics;
using vk.Utils;

namespace vk {
   public static class SingleInstance {
      public static readonly int WM_SHOWFIRSTINSTANCE = AppWinApi.RegisterWindowMessage("WM_SHOWFIRSTINSTANCE|1{0}", ProgramInfo.AssemblyGuid);

      public static bool IsOnlyInstance() {
         var currentProcess = Process.GetCurrentProcess();
         var processes = Process.GetProcessesByName(currentProcess.ProcessName);
         return processes.None(p => p.Id != currentProcess.Id);
      }

      public static void ShowFirstInstance() {
         AppWinApi.PostMessage((IntPtr)AppWinApi.HWND_BROADCAST, WM_SHOWFIRSTINSTANCE, IntPtr.Zero, IntPtr.Zero);
      }
   }
}