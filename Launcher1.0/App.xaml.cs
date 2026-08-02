using System;
using System.Windows;

namespace Launcher1._0
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Catches non-UI background thread crashes
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                Exception ex = args.ExceptionObject as Exception;
                MessageBox.Show($"Fatal Crash:\n{ex?.Message}\n\n{ex?.StackTrace}", "Launcher Error");
            };

            // Catches UI thread crashes
            DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show($"UI Crash:\n{args.Exception.Message}\n\n{args.Exception.StackTrace}", "Launcher Error");
                args.Handled = true; // Prevents app from closing
            };

            base.OnStartup(e);
        }
    }
}