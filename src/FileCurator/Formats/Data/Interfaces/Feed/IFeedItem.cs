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
    /// Feed item
    /// </summary>
    public interface IFeedItem
    {
        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author.</value>
        string? Author { get; set; }

        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        /// <value>The categories.</value>
        IList<string> Categories { get; }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        string Content { get; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        string? Description { get; set; }

        /// <summary>
        /// Gets or sets the enclosure.
        /// </summary>
        /// <value>The enclosure.</value>
        IEnclosure? Enclosure { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>The unique identifier.</value>
        IFeedGuid? GUID { get; set; }

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
        /// Gets or sets the thumbnail.
        /// </summary>
        /// <value>The thumbnail.</value>
        IThumbnail? Thumbnail { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        string? Title { get; set; }
    }
}