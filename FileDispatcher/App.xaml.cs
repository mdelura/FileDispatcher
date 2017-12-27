using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using FileDispatcher.Core;
using FileDispatcher.ViewModels;
using Hardcodet.Wpf.TaskbarNotification;
using NLog;

namespace FileDispatcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static Logger errorsLogger = LogManager.GetLogger("ErrorsLog");

        private bool _initialized;
        private Mutex _mutex;
        private TaskbarIcon _appTray;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            bool createdNew = true;

            var assemblyGuidAttrib = (GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), true)[0];
            _mutex = new Mutex(true, assemblyGuidAttrib.Value, out createdNew);
            if (createdNew)
            {
                _appTray = (TaskbarIcon)FindResource("TrayIcon");
                _appTray.DataContext = new AppTrayViewModel(this);
                _initialized = true;

                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                DispatchManager.Initialize();
            }
            else
            {
                Current.Shutdown();
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            string message = $"Message: {ex.Message}\r\nStackTrace:\r\n{ex.StackTrace}";

            errorsLogger.Error(message);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_initialized)
            {
                _appTray.Dispose();
            }
            _mutex.Dispose();
            base.OnExit(e);
        }
    }
}
