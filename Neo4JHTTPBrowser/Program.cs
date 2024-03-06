using Neo4JHTTPBrowser.Helpers;
using Neo4JHTTPBrowser.Properties;
using System;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace Neo4JHttpBrowser
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Register exception handlers.
            AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
            Application.ThreadException += Application_ThreadException;

            // Configure log4net.
            log4net.Config.XmlConfigurator.Configure();

            // Ignore HTTPS certificate validation.
            if (!Settings.Default.Neo4JVerifySsl)
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        private static void HandleException(Exception ex)
        {
            if (ex == null)
            {
                return;
            }

            var cause = ExceptionHelper.GetCause(ex);
            var message = LogHelper.Fatal(cause);

            MessageBoxHelper.Error(null, message);
        }
    }
}
