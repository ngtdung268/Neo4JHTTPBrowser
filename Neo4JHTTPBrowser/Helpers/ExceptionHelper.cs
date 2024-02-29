using System;

namespace Neo4JHTTPBrowser.Helpers
{
    internal static class ExceptionHelper
    {
        /// <summary>
        /// Get the most-inner exception of the given exception.
        /// </summary>
        /// <param name="ex">The exception to be checked.</param>
        /// <returns>The most-inner exception.</returns>
        public static Exception GetCause(Exception ex)
        {
            if (ex == null)
            {
                return null;
            }

            var innerEx = ex;

            while (innerEx.InnerException != null)
            {
                innerEx = innerEx.InnerException;
            }

            return innerEx;
        }

        /// <summary>
        /// Get the root cause message from the given exception.
        /// </summary>
        /// <param name="ex">The exception to be checked.</param>
        /// <returns>The root cause message.</returns>
        public static string GetCauseMessage(Exception ex)
        {
            if (ex == null)
            {
                return string.Empty;
            }

            var cause = GetCause(ex);

            return cause.Message;
        }

        /// <summary>
        /// Get the root cause message and return the cause exception.
        /// </summary>
        /// <param name="ex">The exception to be checked.</param>
        /// <param name="cause">The root cause exception.</param>
        /// <returns>The root cause message.</returns>
        public static string GetCauseMessage(Exception ex, out Exception cause)
        {
            if (ex == null)
            {
                cause = null;
                return string.Empty;
            }

            cause = GetCause(ex);

            return cause == null ? string.Empty : cause.Message;
        }
    }
}
