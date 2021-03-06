﻿using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.Unity;
using NLog;
using Prism.Mvvm;

namespace vk {
   public partial class App : Application {
      private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

      public static IUnityContainer Container { get; private set; }
      public static bool IsInitialized { get; set; }
      public static Version Version { get; private set; }

      protected override void OnStartup(StartupEventArgs e) {
         Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

         logger.Debug($"VK Postpone Helper ({Version}) пытается запуститься");

         if (SingleInstance.IsOnlyInstance() == false) {
            //SingleInstance.ShowFirstInstance();
            logger.Debug("Уже обнаружена запущенная версия программы. Завершение текущего экземпляра.");
            Current.Shutdown();
            return;
         }

         base.OnStartup(e);

         Current.DispatcherUnhandledException += CurrentOnDispatcherUnhandledException;
         Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

         var bs = new Bootstrapper();

         ViewModelLocationProvider.SetDefaultViewModelFactory(type => bs.Container.Resolve(type));

         bs.Run();
         Container = bs.Container;

         logger.Debug($"VK Postpone Helper версии {Version} запущен.");
      }

      //[System.Runtime.InteropServices.DllImport("wininet.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
      //public static extern bool InternetSetOption(int hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

      //public static unsafe void SuppressWininetBehavior() {
      //   /* SOURCE: http://msdn.microsoft.com/en-us/library/windows/desktop/aa385328%28v=vs.85%29.aspx
      //       * INTERNET_OPTION_SUPPRESS_BEHAVIOR (81):
      //       *      A general purpose option that is used to suppress behaviors on a process-wide basis. 
      //       *      The lpBuffer parameter of the function must be a pointer to a DWORD containing the specific behavior to suppress. 
      //       *      This option cannot be queried with InternetQueryOption. 
      //       *      
      //       * INTERNET_SUPPRESS_COOKIE_PERSIST (3):
      //       *      Suppresses the persistence of cookies, even if the server has specified them as persistent.
      //       *      Version:  Requires Internet Explorer 8.0 or later.
      //       */

      //   int option = (int)3/* INTERNET_SUPPRESS_COOKIE_PERSIST*/;
      //   int* optionPtr = &option;

      //   bool success = InternetSetOption(0, 81/*INTERNET_OPTION_SUPPRESS_BEHAVIOR*/, new IntPtr(optionPtr), sizeof(int));
      //   if (!success) {
      //      MessageBox.Show("Something went wrong !>?");
      //   }
      //}

      private void CurrentOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
         logger.Fatal(e.Exception, "!!Необработанное исключение!! Завершение программы.");
         MessageBox.Show($"{e.Exception.Message}\n\n See dump at Windows Application Event Log.\nStack Trace:{e.Exception.StackTrace}", 
            e.Exception.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);

#if !DEBUG
         e.Handled = true;
#endif

         Environment.FailFast("Exception occured", e.Exception);
      }
   }
}
