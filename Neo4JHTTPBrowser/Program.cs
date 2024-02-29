using Neo4JHTTPBrowser.Helpers;
using System;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace Neo4JHttpBrowser
{
    internal static class Program
    {
        private const string AppGlobalId = "Global\\893d78f8-46e8-4563-aa86-64bf9b8291dd";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Register exception handlers.
            AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
            Application.ThreadException += Application_ThreadException;

            // Only allow running 1 instance of this app.
            using (var m = new Mutex(true, AppGlobalId, out bool isFirstInstance))
            {
                if (!isFirstInstance)
                {
                    MessageBoxHelper.Info(null, "The application is already running.");
                    Application.Exit();
                    return;
                }

                // Configure log4net.
                log4net.Config.XmlConfigurator.Configure();

                // Ignore HTTPS certificate validation.
                if (!AppConfigHelper.Neo4JVerifySsl)
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
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
