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

using System;
using System.Collections.Generic;

namespace FileCurator.Formats.Data.Interfaces
{
    /// <summary>
    /// Channel interface
    /// </summary>
    public interface IChannel : IList<IFeedItem>
    {
        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <value>The categories.</value>
        IList<string> Categories { get; }

        /// <summary>
        /// Gets the cloud.
        /// </summary>
        /// <value>The cloud.</value>
        string? Cloud { get; set; }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        string Content { get; }

        /// <summary>
        /// Gets the copyright.
        /// </summary>
        /// <value>The copyright.</value>
        string Copyright { get; set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        string? Description { get; set; }

        /// <summary>
        /// Gets the docs.
        /// </summary>
        /// <value>The docs.</value>
        string Docs { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IChannel"/> is explicit.
        /// </summary>
        /// <value><c>true</c> if explicit; otherwise, <c>false</c>.</value>
        bool Explicit { get; set; }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        /// <value>The image URL.</value>
        string? ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        string Language { get; set; }

        /// <summary>
        /// Gets or sets the link.
        /// </summary>
        /// <value>The link.</value>
        string? Link { get; set; }

        /// <summary>
        /// Gets or sets the pub date.
        /// </summary>
        /// <value>The pub date.</value>
        DateTime PubDate { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        string? Title { get; set; }

        /// <summary>
        /// Gets or sets the TTL.
        /// </summary>
        /// <value>The TTL.</value>
        int TTL { get; set; }

        /// <summary>
        /// Gets or sets the web master.
        /// </summary>
        /// <value>The web master.</value>
        string? WebMaster { get; set; }
    }
}