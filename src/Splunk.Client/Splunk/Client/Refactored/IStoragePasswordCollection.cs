namespace Splunk.Client.Refactored
{
    using Splunk.Client;

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    [ContractClass(typeof(IStoragePasswordCollectionContract<>))]
    public interface IStoragePasswordCollection<TStoragePassword> : IPaginated, IEntityCollection<TStoragePassword>
        where TStoragePassword : ResourceEndpoint, IStoragePassword, new()
    {
        /// <summary>
        /// Asynchronously creates a new <see cref="StoragePassword"/>.
        /// </summary>
        /// <param name="password">
        /// Password to be stored.
        /// </param>
        /// <param name="name">
        /// A name for the password to be stored.
        /// </param>
        /// <param name="realm">
        /// Optional domain or realm name for the password to be stored.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/JgyIeN">POST 
        /// storage/passwords</a> endpoint to create a <see cref=
        /// "StoragePassword"/> identified by <paramref name="name"/> and
        /// <paramref name="realm"/>.
        /// </remarks>
        Task<TStoragePassword> CreateAsync(string password, string name, string realm);

        Task GetSliceAsync(StoragePasswordCollection.Filter criteria);
    }

    [ContractClassFor(typeof(IStoragePasswordCollection<>))]
    abstract class IStoragePasswordCollectionContract<TStoragePassword> : IStoragePasswordCollection<TStoragePassword>
        where TStoragePassword : ResourceEndpoint, IStoragePassword, new()
    {

        public abstract TStoragePassword this[int index] { get; }
        public abstract int Count { get; }
        public abstract IReadOnlyList<Message> Messages { get; }
        public abstract Pagination Pagination { get; }

        public Task<TStoragePassword> CreateAsync(string password, string name, string realm)
        {
            Contract.Requires<ArgumentException>(password != null);
            Contract.Requires<ArgumentException>(name != null);
            return default(Task<TStoragePassword>);
        }

        public abstract Task GetAllAsync();

        public abstract Task<TStoragePassword> GetAsync(string name);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return default(IEnumerator);
        }

        public abstract IEnumerator<TStoragePassword> GetEnumerator();

        public abstract Task GetSliceAsync(params Argument[] arguments);

        public abstract Task GetSliceAsync(IEnumerable<Argument> arguments);

        public Task GetSliceAsync(StoragePasswordCollection.Filter criteria)
        {
            Contract.Requires<ArgumentException>(criteria != null);
            return default(Task);
        }
        
        public abstract Task ReloadAsync();
    }
}
