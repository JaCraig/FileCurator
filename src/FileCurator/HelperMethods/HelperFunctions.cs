/*
Copyright 2016 James Craig

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System.IO;
using System.Text;

namespace FileCurator.HelperMethods
{
    /// <summary>
    /// Helper functions for directory/file names.
    /// </summary>
    internal static class HelperFunctions
    {
        /// <summary>
        /// Gets the last x number of characters from the right hand side
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="length">x number of characters to return</param>
        /// <returns>The resulting string</returns>
        public static string Left(this string input, int length)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            if (length <= 0)
                return "";
            length = input.Length > length ? length : input.Length;
            return input.Substring(0, length);
        }

        /// <summary>
        /// Removes illegal characters from a directory
        /// </summary>
        /// <param name="directoryName">Directory name</param>
        /// <param name="replacementChar">Replacement character</param>
        /// <returns>DirectoryName with all illegal characters replaced with ReplacementChar</returns>
        public static string RemoveIllegalDirectoryNameCharacters(this string directoryName, char replacementChar = '_')
        {
            if (string.IsNullOrEmpty(directoryName))
                return directoryName;
            var InvalidChars = Path.GetInvalidPathChars();
            for (int i = 0, maxLength = InvalidChars.Length; i < maxLength; i++)
            {
                char Char = InvalidChars[i];
                directoryName = directoryName.Replace(Char, replacementChar);
            }

            return directoryName;
        }

        /// <summary>
        /// Removes illegal characters from a file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="replacementChar">Replacement character</param>
        /// <returns>FileName with all illegal characters replaced with ReplacementChar</returns>
        public static string RemoveIllegalFileNameCharacters(this string fileName, char replacementChar = '_')
        {
            if (string.IsNullOrEmpty(fileName))
                return fileName;
            var InvalidChars = Path.GetInvalidFileNameChars();
            for (int i = 0, InvalidCharsLength = InvalidChars.Length; i < InvalidCharsLength; i++)
            {
                char Char = InvalidChars[i];
                fileName = fileName.Replace(Char, replacementChar);
            }

            return fileName;
        }

        /// <summary>
        /// Gets the last x number of characters from the right hand side
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="length">x number of characters to return</param>
        /// <returns>The resulting string</returns>
        public static string Right(this string input, int length)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            if (length <= 0)
                return "";
            length = input.Length > length ? length : input.Length;
            return input.Substring(input.Length - length, length);
        }

        /// <summary>
        /// Converts a string to a byte array
        /// </summary>
        /// <param name="input">input string</param>
        /// <param name="encodingUsing">The type of encoding the string is using (defaults to UTF8)</param>
        /// <returns>the byte array representing the string</returns>
        public static byte[] ToByteArray(this string input, Encoding encodingUsing = null)
        {
            encodingUsing = encodingUsing ?? Encoding.UTF8;
            return string.IsNullOrEmpty(input) ? new byte[0] : encodingUsing.GetBytes(input);
        }

        /// <summary>
        /// Converts a byte array to a string
        /// </summary>
        /// <param name="input">input array</param>
        /// <param name="encodingUsing">The type of encoding the string is using (defaults to UTF8)</param>
        /// <param name="index">Index to start at</param>
        /// <param name="count">
        /// Number of bytes starting at the index to convert (use -1 for the entire array starting at
        /// the index)
        /// </param>
        /// <returns>string of the byte array</returns>
        public static string ToString(this byte[] input, Encoding encodingUsing, int index = 0, int count = -1)
        {
            if (input == null)
                return "";
            if (count == -1)
                count = input.Length - index;
            encodingUsing = encodingUsing ?? Encoding.UTF8;
            return encodingUsing.GetString(input, index, count);
        }
    }
}