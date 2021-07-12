using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace FileCurator.HelperMethods
{
    /// <summary>
    /// Internal HTTP Client Factory
    /// </summary>
    /// <seealso cref="System.IDisposable"/>
    /// <seealso cref="IDisposable"/>
    public class InternalHttpClientFactory : IDisposable
    {
        /// <summary>
        /// Gets the lock object.
        /// </summary>
        /// <value>The lock object.</value>
        private static object LockObj { get; } = new object();

        /// <summary>
        /// Gets or sets the clients.
        /// </summary>
        /// <value>The clients.</value>
        private Dictionary<Credentials, HttpClient> Clients { get; } = new Dictionary<Credentials, HttpClient>();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var Item in Clients)
            {
                Item.Value?.Dispose();
            }
            Clients.Clear();
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <returns></returns>
        public HttpClient GetClient(Credentials? credentials)
        {
            credentials ??= Credentials.NoCredentials;
            if (Clients.TryGetValue(credentials, out var Result))
                return Result;
            lock (LockObj)
            {
                if (Clients.TryGetValue(credentials, out Result))
                    return Result;
                var Handler = new HttpClientHandler();
                if (!string.IsNullOrEmpty(credentials?.UserName) && !string.IsNullOrEmpty(credentials?.Password))
                {
                    if (!string.IsNullOrEmpty(credentials?.Domain))
                        Handler.Credentials = new NetworkCredential(credentials?.UserName, credentials?.Password, credentials?.Domain);
                    else
                        Handler.Credentials = new NetworkCredential(credentials?.UserName, credentials?.Password);
                }
                else
                {
                    Handler.UseDefaultCredentials = credentials?.UseDefaultCredentials ?? false;
                }
                Result = new HttpClient(Handler);
                Clients.Add(credentials, Result);
            }
            return Result;
        }
    }
}