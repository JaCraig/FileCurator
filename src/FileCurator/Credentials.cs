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

namespace FileCurator
{
    /// <summary>
    /// Credentials info
    /// </summary>
    public class Credentials
    {
        /// <summary>
        /// Gets the no credentials.
        /// </summary>
        /// <value>The no credentials.</value>
        public static Credentials NoCredentials => new Credentials();

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        /// <value>The domain.</value>
        public string? Domain { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use default credentials].
        /// </summary>
        /// <value><c>true</c> if [use default credentials]; otherwise, <c>false</c>.</value>
        public bool UseDefaultCredentials { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string? UserName { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Credentials CredentialsObj))
                return false;
            return UserName == CredentialsObj.UserName
                && Password == CredentialsObj.Password
                && UseDefaultCredentials == CredentialsObj.UseDefaultCredentials
                && Domain == CredentialsObj.Domain;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(UserName, Password, Domain, UseDefaultCredentials);
        }
    }
}