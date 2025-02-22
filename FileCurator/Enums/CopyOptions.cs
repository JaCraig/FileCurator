﻿/*
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

namespace FileCurator.Enums
{
    /// <summary>
    /// Options used in directory copying
    /// </summary>
    public enum CopyOptions
    {
        /// <summary>
        /// Copy if newer than the DateTime specified
        /// </summary>
        CopyIfNewer,

        /// <summary>
        /// Copy always
        /// </summary>
        CopyAlways,

        /// <summary>
        /// Do not overwrite a file
        /// </summary>
        DoNotOverwrite
    }
}