using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seva.Assessment.DataService
{
    public interface ILogger
    {
        /// <summary>
        /// Logs an error to errorlog file.
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <param name="message">The message.</param>
        void LogError(Exception ex, string message);

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="wrapperType">Type of the wrapper.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="message">The message.</param>
        void LogError(Type wrapperType, Exception ex, string message);
    }

    public class Logger : ILogger
    {
        private readonly NLog.Logger _nloggerInstance;

        /// <summary>
        /// Initializes the <see cref="Logger"/> class.
        /// </summary>
        public Logger()
        {
            var logfile = new NLog.Targets.FileTarget
            {
                //TODO missing config management.
                FileName = "errorlog.txt",
                Name = "logfile",
                Layout = @"${date:format=dd.MM.yyyy HH\:mm\:ss.fff} ${callsite}(line ${callsite-linenumber}): ${exception:format=toString,Data:maxInnerExceptionLevel=10}: ${message} : Callstack: ${stacktrace}"
            };
            var config = new NLog.Config.LoggingConfiguration();
            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", LogLevel.Error, logfile));
            LogManager.Configuration = config;
            _nloggerInstance = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Logs an error to errorlog file.
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <param name="message">The message.</param>
        public void LogError(Exception ex, string message)
        {
            LogError(typeof(Logger), ex, message);
        }

        /// <summary>
        /// Logs an error to errorlog file.
        /// </summary>
        /// <param name="wrapperType">Type of the nlog wrapper.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="message">The message.</param>
        public void LogError(Type wrapperType, Exception ex, string message)
        {
            var logEvent = new LogEventInfo(LogLevel.Error, _nloggerInstance.Name, message)
            {
                Exception = ex
            };
            _nloggerInstance.Log(wrapperType, logEvent);
        }
    }
}
