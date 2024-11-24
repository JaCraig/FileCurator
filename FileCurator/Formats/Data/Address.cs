using FileCurator.Formats.Data.Interfaces;

namespace FileCurator.Formats.Data
{
    /// <summary>
    /// Address class
    /// </summary>
    /// <seealso cref="IAddress"/>
    public class Address : IAddress
    {
        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>The city.</value>
        public string? City { get; set; }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>The country.</value>
        public string? Country { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the state or provence.
        /// </summary>
        /// <value>The state or provence.</value>
        public string? StateOrProvence { get; set; }

        /// <summary>
        /// Gets or sets the street.
        /// </summary>
        /// <value>The street.</value>
        public string? Street { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the area code.
        /// </summary>
        /// <value>The area code.</value>
        public string? ZipCode { get; set; }
    }
}