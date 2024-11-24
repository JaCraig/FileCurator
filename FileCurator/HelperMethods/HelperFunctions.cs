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

using System;
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
                var Char = InvalidChars[i];
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
                var Char = InvalidChars[i];
                fileName = fileName.Replace(Char, replacementChar);
            }

            return fileName;
        }

        /// <summary>
        /// Converts a string to a byte array
        /// </summary>
        /// <param name="input">input string</param>
        /// <param name="encodingUsing">The type of encoding the string is using (defaults to UTF8)</param>
        /// <returns>the byte array representing the string</returns>
        public static byte[] ToByteArray(this string input, Encoding? encodingUsing = null)
        {
            encodingUsing ??= Encoding.UTF8;
            return string.IsNullOrEmpty(input) ? Array.Empty<byte>() : encodingUsing.GetBytes(input);
        }
    }
}