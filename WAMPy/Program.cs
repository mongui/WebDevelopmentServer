using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace WDS
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Any error nobody caught ends in the log (with its stack trace) instead of being lost.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) =>
            {
                Globals.LogError("Unexpected error", e.Exception);
                MessageBox.Show("Unexpected error:\r\n" + e.Exception.Message + "\r\n\r\nThe details are in the logs folder.", "Web Development Server");
            };
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Globals.LogError("Fatal error" + (e.IsTerminating ? " (the program closes)" : ""), e.ExceptionObject as Exception);
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
