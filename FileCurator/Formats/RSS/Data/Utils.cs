/*
Copyright 2017 James Craig

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

namespace FileCurator.Formats.RSS.Data
{
    /// <summary>
    /// Utility class used by RSS classes.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Strips illegal characters from RSS items
        /// </summary>
        /// <param name="original">Original text</param>
        /// <returns>string stripped of certain characters.</returns>
        public static string StripIllegalCharacters(string original)
        {
            return original?.Replace("&nbsp;", " ")
                .Replace("&#160;", string.Empty)
                .Trim()
                .Replace("&", "and") ?? "";
        }
    }
}