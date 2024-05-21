using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CaelumServer.Utilitatem
{

    internal static partial class Extensions
    {
        public static bool _IsDefined<T>(string name) where T : struct
        {
            return Enum.IsDefined(typeof(T), name);
        }

        public static bool _IsDefined<T>(T value) where T : struct
        {
            return Enum.IsDefined(typeof(T), value);
        }

        public static IEnumerable<T> _GetValues<T>() where T : struct
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static T _GetDefault<T>() where T : struct
        {
            return Enum.GetValues(typeof(T)).Cast<T>().FirstOrDefault();
        }

        public static bool _ContainsOrDefault<T>(this IEnumerable<T> list, T compare) where T : struct
        {

            if (list == null || !list.Any())
                list = new List<T>() { _GetDefault<T>() };

            return list.Any(x => x.Equals(compare) || x.Equals(_GetDefault<T>()));
        }

        public static string _GetDescription(this Enum value)
        {
            var attribute = value._GetAttribute<DescriptionAttribute>();

            return attribute.Description;
        }

        public static T _GetAttribute<T>(this Enum value) where T : Attribute, new()
        {
            var type = value.GetType();

            var memberInfo = type.GetMember(value.ToString());

            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);

            if (attributes == null || !attributes.Any())
                return new T();

            return (T)attributes[0];
        }

        public static T _ToEnum<T>(this string value) where T : struct
        {
            try
            {
                T enumValue;

                if (!Enum.TryParse(value, true, out enumValue))
                {
                    return default(T);
                }

                return enumValue;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static T _ToEnum<T>(this string value, T defaultValue) where T : struct
        {
            try
            {
                T enumValue;

                if (!Enum.TryParse(value, true, out enumValue))
                {
                    return defaultValue;
                }

                return enumValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static string _EnumtoString<T>(this int value) where T : struct
        {
            try
            {
                var result = default(T);

                bool success = Enum.IsDefined(typeof(T), value);

                if (success)
                {
                    result = (T)Enum.ToObject(typeof(T), value);
                }

                return result.ToString();
            }
            catch (Exception)
            {
                return default(T).ToString();
            }
        }

    }
}
