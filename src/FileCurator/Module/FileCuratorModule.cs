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

using Canister.Interfaces;
using FileCurator.Formats.Interfaces;
using FileCurator.HelperMethods;
using FileCurator.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FileCurator.Module
{
    /// <summary>
    /// File curator module
    /// </summary>
    public class FileCuratorModule : IModule
    {
        /// <summary>
        /// Order to run it in
        /// </summary>
        public int Order => 0;

        /// <summary>
        /// Loads the module
        /// </summary>
        /// <param name="bootstrapper">The bootstrapper.</param>
        public void Load(IServiceCollection? bootstrapper)
        {
            bootstrapper?.AddAllTransient<IFileSystem>()
                ?.AddSingleton<FileSystem>()
                .AddAllSingleton<IFormat>()
                ?.AddSingleton<Formats.Manager>()
                .AddSingleton<InternalHttpClientFactory>();
        }
    }
}