// NAnt - A .NET build tool
// Copyright (C) 2001-2003 Gerry Shaw
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Ian Maclean (ian_maclean@another.com)
// Jaroslaw Kowalski (jkowalski@users.sourceforge.net)

using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Globalization;

using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;

namespace NAnt.Core.Functions {
    [FunctionSet("string", "String")]
    public class StringFunctions : FunctionSetBase {
        #region Public Instance Constructors

        public StringFunctions(Project project, PropertyDictionary propDict) : base(project, propDict) {
        }

        #endregion Public Instance Constructors

        #region Public Static Methods

        /// <summary>
        /// Returns the length of the specified string.
        /// </summary>
        /// <param name="s">input string</param>
        /// <returns>
        /// The string's length.
        /// </returns>
        /// <example>
        /// <code>string::get-length('foo') ==> 3</code>
        /// <code>string::get-length('') ==> 0</code>
        /// </example>
        [Function("get-length")]
        public static int GetLength(string s) {
            return s.Length;
        }

        /// <summary>
        /// Returns a substring of the specified string.
        /// </summary>
        /// <param name="str">input string</param>
        /// <param name="start">position of the start of the substring</param>
        /// <param name="length">the length of the substring</param>
        /// <returns>
        /// <para>
        /// If the <paramref name="length" /> is greater than zero, the
        /// function returns a substring starting at character position
        /// <paramref name="start" /> with a length of <paramref name="length" />
        /// characters.
        /// </para>
        /// <para>
        /// If the <paramref name="length" /> is less than zero, the returned
        /// substring will start at character position <paramref name="start" />
        /// and continue to the end of <paramref name="str" />.
        /// </para>
        /// <para>
        /// If the <paramref name="length" /> is equal to zero, the function
        /// returns an empty string.
        /// </para>
        /// </returns>
        /// <example>
        /// <code>string::substring('testing string', 0, 4) ==> 'test'</code>
        /// <code>string::substring('testing string', 8, 3) ==> 'str'</code>
        /// <code>string::substring('testing string', 8, 0) ==> ''</code>
        /// <code>string::substring('testing string', 8, -1) ==> 'string'</code>
        /// </example>
        [Function("substring")]
        public static string Substring(string str, int start, int length) {
            if (length < 0) {
                return str.Substring(start);
            } else {
                return str.Substring(start, length);
            }
        }

        /// <summary>
        /// Tests whether the specified string starts with the specified prefix
        /// string.
        /// </summary>
        /// <param name="s1">test string</param>
        /// <param name="s2">prefix string</param>
        /// <returns>
        /// <see langword="true" /> when <paramref name="s2" /> is a prefix for
        /// the string <paramref name="s1" />. Meaning, the characters at the 
        /// beginning of <paramref name="s1" /> are identical to
        /// <paramref name="s2" />; otherwise, <see langword="false" />.
        /// </returns>
        /// <example>
        /// <code>string::starts-with('testing string', 'test') ==> true</code>
        /// <code>string::starts-with('testing string', 'testing') ==> true</code>
        /// <code>string::starts-with('testing string', 'string') ==> false</code>
        /// <code>string::starts-with('test', 'testing string') ==> false</code>
        /// </example>
        [Function("starts-with")]
        public static bool StartsWith(string s1, string s2) {
            return s1.StartsWith(s2);
        }

        /// <summary>
        /// Tests whether the specified string ends with the specified suffix
        /// string.
        /// </summary>
        /// <param name="s1">test string</param>
        /// <param name="s2">suffix string</param>
        /// <returns>
        /// <see langword="true" /> when <paramref name="s2" /> is a suffix for
        /// the string <paramref name="s1" />. Meaning, the characters at the 
        /// end of <paramref name="s1" /> are identical to 
        /// <paramref name="s2" />; otherwise, <see langword="false" />.
        /// </returns>
        /// <example>
        /// <code>string::ends-with('testing string', 'string') ==> true</code>
        /// <code>string::ends-with('testing string', '') ==> true</code>
        /// <code>string::ends-with('testing string', 'bring') ==> false</code>
        /// <code>string::ends-with('string', 'testing string') ==> false</code>
        /// </example>
        [Function("ends-with")]
        public static bool EndsWith(string s1, string s2) {
            return s1.EndsWith(s2);
        }

        /// <summary>
        /// Returns the specified string converted to lower case.
        /// </summary>
        /// <param name="s">input string</param>
        /// <returns>
        /// The string <paramref name="s" /> converted to lower case.
        /// </returns>
        /// <example>
        /// <code>string::to-lower('testing string') ==> 'testing string'</code>
        /// <code>string::to-lower('Testing String') ==> 'testing string'</code>
        /// <code>string::to-lower('Test 123') ==> 'test 123'</code>
        /// </example>
        [Function("to-lower")]
        public static string ToLower(string s) {
            return s.ToLower(CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Returns the specified string converted to upper case.
        /// </summary>
        /// <param name="s">input string</param>
        /// <returns>
        /// The string <paramref name="s" /> converted to upper case.
        /// </returns>
        /// <example>
        /// <code>string::to-upper('testing string') ==> 'TESTING STRING'</code>
        /// <code>string::to-upper('Testing String') ==> 'TESTING STRING'</code>
        /// <code>string::to-upper('Test 123') ==> 'TEST 123'</code>
        /// </example>
        [Function("to-upper")]
        public static string ToUpper(string s) {
            return s.ToUpper(CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Returns a string corresponding to the replacement of a given string
        /// with another in the specified string.
        /// </summary>
        /// <param name="str">input string</param>
        /// <param name="strold">search string</param>
        /// <param name="strnew">replacement string</param>
        /// <returns>
        /// The result of replacing every instance of <paramref name="strold" />
        /// in <paramref name="str" /> with <paramref name="strnew" />.
        /// </returns>
        /// <example>
        /// <code>string::replace('testing string', 'test', 'winn') ==> 'winning string'</code>
        /// <code>string::replace('testing string', 'foo', 'winn') ==> 'testing string'</code>
        /// <code>string::replace('testing string', 'ing', '') ==> 'test str'</code>
        /// <code>string::replace('banana', 'ana', 'ana') ==> 'banana'</code>
        /// </example>
        [Function("replace")]
        public static string Replace(string str, string strold, string strnew) {
            return str.Replace(strold, strnew);
        }
        /// <summary>
        /// Tests whether the specified string contains the given search string.
        /// </summary>
        /// <param name="str">input string</param>
        /// <param name="substr">search string</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="substr" /> is found in 
        /// <paramref name="str" />; otherwise, <see langword="false" />.
        /// </returns>
        /// <example>
        /// <code>string::contains('testing string', 'test') ==> true</code>
        /// <code>string::contains('testing string', '') ==> true</code>
        /// <code>string::contains('testing string', 'Test') ==> false</code>
        /// <code>string::contains('testing string', 'foo') ==> false</code>
        /// </example>
        [Function("contains")]
        public static bool Contains(string str, string substr) {
            return str.IndexOf(substr) >= 0;
        }

        /// <summary>
        /// Returns the position of the first occurrence in the specified string
        /// of the given search string.
        /// </summary>
        /// <param name="str">input string</param>
        /// <param name="substr">search string</param>
        /// <returns>
        /// <para>
        /// The lowest-index position of <paramref name="substr" /> in
        /// <paramref name="str" /> if it is found, or -1 if <paramref name="str" /> 
        /// does not contain <paramref name="substr" />.
        /// </para>
        /// <para>
        /// If <paramref name="substr" /> is an empty string, the return value
        /// will always be <c>0</c>.
        /// </para>
        /// </returns>
        /// <example>
        /// <code>string::index-of('testing string', 'test') ==> 0</code>
        /// <code>string::index-of('testing string', '') ==> 0</code>
        /// <code>string::index-of('testing string', 'Test') ==> -1</code>
        /// <code>string::index-of('testing string', 'ing') ==> 4</code>
        /// </example>
        [Function("index-of")]
        public static int IndexOf(string str, string substr) {
            return str.IndexOf(substr);
        }

        /// <summary>
        /// Returns the position of the last occurrence in the specified string
        /// of the given search string.
        /// </summary>
        /// <param name="str">input string</param>
        /// <param name="substr">search string</param>
        /// <returns>
        /// <para>
        /// The highest-index position of <paramref name="substr" /> in
        /// <paramref name="str" /> if it is found, or -1 if <paramref name="str" /> 
        /// does not contain <paramref name="substr" />.
        /// </para>
            /// <para>
        /// If <paramref name="substr" /> is an empty string, the return value
        /// will always be <c>0</c>.
        /// </para>
        /// </returns>
        /// <example>
        /// <code>string::last-index-of('testing string', 'test') ==> 0</code>
        /// <code>string::last-index-of('testing string', '') ==> 0</code>
        /// <code>string::last-index-of('testing string', 'Test') ==> -1</code>
        /// <code>string::last-index-of('testing string', 'ing') ==> 4</code>
        /// </example>
        [Function("last-index-of")]
        public static int LastIndexOf(string str, string substr) {
            return str.LastIndexOf(substr);
        }

        /// <summary>
        /// Returns the given string left-padded to the given length.
        /// </summary>
        /// <param name="s">input string</param>
        /// <param name="width">required length</param>
        /// <param name="paddingChar">string containing padding character</param>
        /// <returns>
        /// If the length of <paramref name="s" /> is at least 
        /// <paramref name="width" />, then it is returned. Otherwise,
        /// <paramref name="s" /> will be continually prepended with the padding
        /// character obtained from <paramref name="paddingChar" /> until it
        /// is of the required length.
        /// </returns>
        /// <remarks>
        /// Note that only the first character of <paramref name="paddingChar" />
        /// will be used when padding the result.
        /// </remarks>
        /// <example>
        /// <code>string::pad-left('test', 10, ' ') ==> '      test'</code>
        /// <code>string::pad-left('test', 10, 'test') ==> 'tttttttest'</code>
        /// <code>string::pad-left('test', 3, ' ') ==> 'test'</code>
        /// </example>
        [Function("pad-left")]
        public static string PadLeft(string s, int width, string paddingChar) {
            return s.PadLeft(width, paddingChar[0]);
        }

        /// <summary>
        /// Returns the given string right-padded to the given length.
        /// </summary>
        /// <param name="s">input string</param>
        /// <param name="width">required length</param>
        /// <param name="paddingChar">string containing padding character</param>
        /// <returns>
        /// If the length of <paramref name="s" /> is at least 
        /// <paramref name="width" />, then it is returned. Otherwise,
        /// <paramref name="s" /> will be continually appended with the padding
        /// character obtained from <paramref name="paddingChar" /> until it
        /// is of the required length.
        /// </returns>
        /// <remarks>
        /// Note that only the first character of <paramref name="paddingChar" />
        /// will be used when padding the result.
        /// </remarks>
        /// <example>
        /// <code>string::pad-right('test', 10, ' ') ==> 'test      '</code>
        /// <code>string::pad-right('test', 10, 'abcd') ==> 'testaaaaaa'</code>
        /// <code>string::pad-right('test', 3, ' ') ==> 'test'</code>
        /// </example>
        [Function("pad-right")]
        public static string PadRight(string s, int width, string paddingChar) {
            return s.PadRight(width, paddingChar[0]);
        }

        /// <summary>
        /// Returns the given string trimmed of whitespace.
        /// </summary>
        /// <param name="s">input string</param>
        /// <returns>
        /// The string <paramref name="s" /> with any leading or trailing
        /// whitespace characters removed.
        /// </returns>
        /// <example>
        /// <code>string::trim('  test  ') ==> 'test'</code>
        /// <code>string::trim('\t\tfoo  \r\n') ==> 'foo'</code>
        /// </example>
        [Function("trim")]
        public static string Trim(string s) {
            return s.Trim();
        }
        
        /// <summary>
        /// Returns the given string trimmed of leading whitespace.
        /// </summary>
        /// <param name="s">input string</param>
        /// <returns>
        /// The string <paramref name="s" /> with any leading
        /// whitespace characters removed.
        /// </returns>
        /// <example>
        /// <code>string::trim-start('  test  ') ==> 'test  '</code>
        /// <code>string::trim-start('\t\tfoo  \r\n') ==> 'foo  \r\n'</code>
        /// </example>
        [Function("trim-start")]
        public static string TrimStart(string s) {
            return s.TrimStart();
        }
        
        /// <summary>
        /// Returns the given string trimmed of trailing whitespace.
        /// </summary>
        /// <param name="s">input string</param>
        /// <returns>
        /// The string <paramref name="s" /> with any trailing
        /// whitespace characters removed.
        /// </returns>
        /// <example>
        /// <code>string::trim-end('  test  ') ==> '  test'</code>
        /// <code>string::trim-end('\t\tfoo  \r\n') ==> '\t\tfoo'</code>
        /// </example>
        [Function("trim-end")]
        public static string TrimEnd(string s) {
            return s.TrimEnd();
        }

        #endregion Public Static Methods
    }
}
