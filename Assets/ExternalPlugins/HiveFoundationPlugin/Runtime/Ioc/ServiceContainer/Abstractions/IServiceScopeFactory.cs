﻿namespace Modules.Hive.Ioc
{
    /// <summary>
    /// A factory for creating instances of <see cref="IServiceScope"/>, which is used to create services within a scope.
    /// </summary>
    public interface IServiceScopeFactory
    {
        /// <summary>
        /// Create an <see cref="IServiceScope"/> which contains an <see cref="System.IServiceProvider"/> 
        /// used to resolve dependencies from a newly created scope.
        /// </summary>
        /// <returns>
        /// An <see cref="IServiceScope"/> controlling the lifetime of the scope.
        /// Once this is disposed, any scoped services that have been resolved from the scope will also be disposed.
        /// </returns>
        IServiceScope CreateScope();
    }
}
