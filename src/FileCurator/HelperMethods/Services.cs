using Microsoft.Extensions.DependencyInjection;
using System;

namespace FileCurator.HelperMethods
{
    /// <summary>
    /// Services class
    /// </summary>
    internal static class Services
    {
        /// <summary>
        /// Gets the service provider.
        /// </summary>
        /// <value>The service provider.</value>
        public static IServiceProvider? ServiceProvider
        {
            get
            {
                if (_ServiceProvider is not null)
                    return _ServiceProvider;
                lock (ServiceProviderLock)
                {
                    _ServiceProvider = new ServiceCollection().AddCanisterModules()?.BuildServiceProvider();
                }
                return _ServiceProvider;
            }
        }

        /// <summary>
        /// The service provider lock
        /// </summary>
        private static readonly object ServiceProviderLock = new();

        /// <summary>
        /// The service provider
        /// </summary>
        private static IServiceProvider? _ServiceProvider;
    }
}