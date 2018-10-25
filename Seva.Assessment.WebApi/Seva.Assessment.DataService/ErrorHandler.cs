using NHibernate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seva.Assessment.DataService
{
    public class ErrorHandler
    {
        private static ILogger _logger;

        internal static ILogger Logger => _logger ?? (_logger = new Logger());

        public static TResult Handle<TResult>(Func<TResult> func, bool enableErrorLogging = true)
        {
            return Handle(func, Logger, enableErrorLogging);
        }
        public static TResult Handle<TResult>(Func<TResult> func, ILogger logger, bool enableErrorLogging = true)
        {
            try
            {
                return func();
            }
            catch (ADOException adoException)
            {
                if (enableErrorLogging)
                    Log(logger, adoException);
                throw new HandledException("Database Error");
            }
            catch (HibernateException hibernateException)
            {
                if (enableErrorLogging)
                    Log(logger, hibernateException);
                throw new HandledException("Error with ORM");
            }
            catch (Exception e)
            {
                if (enableErrorLogging)
                    Log(logger, e);
                throw new HandledException("Unknown Exception");
            }
        }

        public static TResult Handle<TResult, TParam>(Func<TParam, TResult> func, TParam param, bool enableErrorLogging = true)
        {
            return Handle(func, Logger, param, enableErrorLogging);
        }
        public static TResult Handle<TResult, TParam>(Func<TParam, TResult> func, ILogger logger, TParam param, bool enableErrorLogging = true)
        {
            try
            {
                return func(param);
            }
            catch (ADOException adoException)
            {
                if (enableErrorLogging)
                    Log(logger, adoException);
                throw new HandledException("Database Error");
            }
            catch (HibernateException hibernateException)
            {
                if (enableErrorLogging)
                    Log(logger, hibernateException);
                throw new HandledException("Error with ORM");
            }
            catch (Exception e)
            {
                if (enableErrorLogging)
                    Log(logger, e);
                throw new HandledException("Unknown Exception");
            }
        }

        public static void Handle(Action action, bool enableErrorLogging = true)
        {
            Handle(action, Logger, enableErrorLogging);
        }

        public static void Handle(Action action, ILogger logger, bool enableErrorLogging = true)
        {
            try
            {
                action();
            }
            catch (ADOException adoException)
            {
                if (enableErrorLogging)
                    Log(logger, adoException);
                throw new HandledException("Database Error");
            }
            catch (HibernateException hibernateException)
            {
                if (enableErrorLogging)
                    Log(logger, hibernateException);
                throw new HandledException("Error with ORM");
            }
            catch (Exception e)
            {
                if (enableErrorLogging)
                    Log(logger, e);
                throw new HandledException("Unknown Exception");
            }
        }

        public static void Handle<T>(Action<T> action, T param, bool enableErrorLogging = true)
        {
            Handle(action, Logger, param, enableErrorLogging);
        }

        public static void Handle<T>(Action<T> action, ILogger logger, T param, bool enableErrorLogging = true)
        {
            try
            {
                action(param);
            }
            catch (ADOException adoException)
            {
                if (enableErrorLogging)
                    Log(logger, adoException);
                throw new HandledException("Database Error");
            }
            catch (HibernateException hibernateException)
            {
                if (enableErrorLogging)
                    Log(logger, hibernateException);
                throw new HandledException("Error with ORM");
            }
            catch (Exception e)
            {
                if (enableErrorLogging)
                    Log(logger, e);
                throw new HandledException("Unknown Exception");
            }
        }

        private static void Log(ILogger logger, Exception exception)
        {
            logger.LogError(typeof(ErrorHandler), exception, exception?.Message);
        }
    }
}
