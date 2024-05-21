using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Serilog;

namespace CaelumServer.Utilitatem
{
    internal static partial class Extensions
    {
        public static string _Replace(this string input, string[] chars, string replacement = null)
        {
            if (chars == null || !chars.Any())
                return input;

            foreach (var _char in chars)
            {
                input = input.Replace(_char, replacement);
            }

            return input;
        }


        /// <summary>
        /// string.Equals with StringComparison.InvariantCultureIgnoreCase
        /// </summary>
        /// <param name="input"></param>
        /// <param name="value"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static bool _Equals(this string input, string value) => string.Equals(input, value, StringComparison.InvariantCultureIgnoreCase);

        public static bool _IsEqual(this string input, string value, StringComparison comp = StringComparison.InvariantCultureIgnoreCase) => string.Equals(input, value, comp);

        public static string _IfNullOrEmpty(this string input, string defaultvalue)
        {
            if (defaultvalue._IsNullOrEmpty())
                defaultvalue = string.Empty;

            return string.IsNullOrEmpty(input) ? defaultvalue : input;
        }

        /// <summary>
        /// Cause I'm tired of typing it!
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool _IsNullOrEmpty(this string input)
        {
            return string.IsNullOrEmpty(input);
        }

        public static bool _IsNotNullOrEmpty(this string input)
        {
            return !string.IsNullOrEmpty(input);
        }

        public static bool _IsNumeric(this string str)
        {
            if (str._IsNullOrEmpty())
                return false;

            return int.TryParse(str, out int i);

        }

        public static bool _Contains(this string source, string toCheck)
        {

            StringComparison comp = StringComparison.OrdinalIgnoreCase;

            return source?.IndexOf(toCheck, comp) >= 0;
        }

        public static bool _Contains(this string source, string toCheck, StringComparison comp = StringComparison.OrdinalIgnoreCase)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }

        public static int _IndexOf(this string str, string find, int count = 0, int startIndex = 0)
        {
            if (str._IsNullOrEmpty())
                return 0;

            if (find == null)
                return 0;

            if (count < 1)
                return str.IndexOf(find, startIndex, StringComparison.InvariantCultureIgnoreCase);

            //if (count < 1)
            // throw new NotSupportedException("Param 'count' must be greater than 0!");

            if (startIndex > 0)
                str = str.Substring(startIndex);

            Match m = Regex.Match(str, "((" + Regex.Escape(find) + ").*?){" + count + "}", RegexOptions.IgnoreCase);

            if (m.Success)
                return m.Groups[2].Captures[count - 1].Index;
            else
                return -1;
        }

        public static string _Clean(this string input)
        {
            return Regex.Replace(input, @"\p{C}+", string.Empty);
        }
        public static string _Summmarise(this string value, int maxChars, string end = "...")
        {
            var output = value._Clean();

            return output.Length <= maxChars ? output : output.Substring(0, maxChars) + end;
        }

        public static string _Truncate(this string value, int maxChars, string end = "...")
        {
            if (string.IsNullOrEmpty(value)) { return value; }

            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + end;
        }

        public static string _Substring(this string str, int startindex, int length = -1)
        {
            if (str._IsNullOrEmpty())
                return str;

            if (startindex < 0 || startindex > str.Length) startindex = 0;

            if (length > 0 && (startindex + length) < str.Length)
            {
                return str.Substring(startindex, length);
            }
            else
            {
                return str.Substring(startindex);
            }

        }

        public static string _Remove(this string input, string find)
        {
            return input.Replace(find, string.Empty);
        }

        public static string _ToStringorEmpty(this object value)
        {
            return value == null ? string.Empty : value.ToString();
        }

        public static string _ToStringorEmpty(this object value, string Default)
        {
            return value == null ? Default : value.ToString();
        }

        public static bool _IsString(this object input)
        {
            Type type = input.GetType();
            return type.Equals(typeof(string));
        }

        public static string _ExtractQuoted(this string source, out IEnumerable<string> extract)
        {
            var _extract = new List<string>();

            foreach (Match match in Regex.Matches(source, "\"([^\"]*)\""))
            {
                var _match = match.ToString();

                _extract.Add(_match);

                source = source._Remove(_match);

            }

            extract = _extract;

            return source;

        }

        public static string _Extract(this string source, string term, out string extract)
        {
            int _start, _end, _length;
            string _extract;

            extract = default(string);

            if (source.Contains(term, StringComparison.InvariantCultureIgnoreCase))
            {
                _length = term.Length;
                _start = source._IndexOf(term);

                if (source.Substring(_start + _length, 1) == "\"")
                {

                    _end = source._IndexOf("\"", count: 2) + 1;
                }
                else
                {
                    _end = source._IndexOf(" ", startIndex: _start);
                }

                _end = (_end > 0) ? _end : source.Length;

                _extract = source.Substring(_start, _end - _start);

                extract = _extract.Split(":").Skip(1).FirstOrDefault()._Remove("\"");

                // Log.Verbose("[Events][Extract] source:{0} _extract:{1} extract:{2}", source, _extract, extract);

                return source.Remove(_start, _end - _start);
            }
            else
            {

                Log.Verbose("[Events][Extract] Source Not Contain term:{0} source:{1} extract:{2}", term, source, extract);

                return source;
            }
        }
    }
}
