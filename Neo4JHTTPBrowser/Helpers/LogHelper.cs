using System;
using System.Reflection;

namespace Neo4JHTTPBrowser.Helpers
{
    internal static class LogHelper
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static string Debug(string message)
        {
            logger.Debug(message);
            return message;
        }

        public static string Info(string message)
        {
            logger.Info(message);
            return message;
        }

        public static string Error(string message)
        {
            logger.Error(message);
            return message;
        }

        public static string Error(Exception ex)
        {
            var message = ExceptionHelper.GetCauseMessage(ex, out Exception cause);
            logger.Error(cause.Message, cause);
            return message;
        }

        public static string Fatal(Exception ex)
        {
            var message = ExceptionHelper.GetCauseMessage(ex, out Exception cause);
            logger.Fatal(cause.Message, cause);
            return message;
        }
    }
}
