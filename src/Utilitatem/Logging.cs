using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

using Serilog;

namespace CaelumServer.Utilitatem.Logging
{
    using Serilog;
    using Serilog.Context;
    using Serilog.Core.Enrichers;

    internal static class Logging
    {
        private const string ClassKey = Constants.CONTEXT_CLASS_KEY;
        private const string MethodKey = Constants.CONTEXT_METHOD_KEY;

        public static ILogger _AddContext<T>(this ILogger logger) where T : class
        {
            return logger._ForContext(ClassKey, typeof(T).Name);
        }

        public static ILogger _AddContext(this ILogger logger, string classname)
        {
            return logger._ForContext(ClassKey, classname);
        }

        public static ILogger _AddContext<T>(this ILogger logger, string methodname) where T : class
        {
            return logger
                  ._ForContext(ClassKey, typeof(T).Name)
                  ._ForContext(MethodKey, methodname);
        }

        public static ILogger _AddContext(this ILogger logger, string classname, string methodname)
        {
            return logger
                ._ForContext(ClassKey, classname)
                ._ForContext(MethodKey, methodname);
        }

        public static ILogger _AddMethodContext(this ILogger logger, [CallerMemberName] string methodname = "")
        {
            return logger._ForContext(MethodKey, methodname);
        }

        public static ILogger _AddMethodContext(this ILogger logger, string classname, [CallerMemberName] string methodname = "")
        {
            return logger
                ._ForContext(ClassKey, classname)
                ._ForContext(MethodKey, methodname);

        }

        public static ILogger _AddMethodContext<T>(this ILogger logger, [CallerMemberName] string methodname = "") where T : class
        {

            return logger
                ._ForContext(ClassKey, typeof(T).Name)
                ._ForContext(MethodKey, methodname);
        }

        private static ILogger _ForContext(this ILogger logger, string key, string value)
        {
            if (value._IsNotNullOrEmpty())
            {
                if (value.StartsWith("["))
                {
                    logger = logger.ForContext(key, value);
                }
                else
                {
                    logger = logger.ForContext(key, $"[{value}]");
                }
            }

            return logger;
        }

        public static IDisposable _PushContext(string classname)
        {
            var _properties = new List<PropertyEnricher>();

            _properties.addPropertyEnricher(ClassKey, classname);

            return LogContext.Push(_properties.ToArray());
        }

        public static IDisposable _PushContext<T>([CallerMemberName] string methodname = "") where T : class
        {
            var _properties = new List<PropertyEnricher>();

            _properties.addPropertyEnricher(ClassKey, typeof(T).Name);

            _properties.addPropertyEnricher(MethodKey, methodname);

            return LogContext.Push(_properties.ToArray());
        }

        public static IDisposable _PushMethodContext<T>([CallerMemberName] string methodname = "")
        {
            var _properties = new List<PropertyEnricher>();

            _properties.addPropertyEnricher(ClassKey, typeof(T).Name);

            _properties.addPropertyEnricher(MethodKey, methodname);

            return LogContext.Push(_properties.ToArray());
        }

        public static IDisposable _PushMethodContext([CallerMemberName] string methodname = "")
        {
            var _properties = new List<PropertyEnricher>();

            _properties.addPropertyEnricher(MethodKey, methodname);

            return LogContext.Push(_properties.ToArray());
        }


        private static IList<PropertyEnricher> addPropertyEnricher(this IList<PropertyEnricher> list, string key, string value)
        {
            if (value._IsNotNullOrEmpty())
            {
                if (value.StartsWith("["))
                {
                    list.Add(new PropertyEnricher(key, value));
                }
                else
                {
                    list.Add(new PropertyEnricher(key, $"[{value}]"));
                }
            }

            return list;
        }
    }
}
