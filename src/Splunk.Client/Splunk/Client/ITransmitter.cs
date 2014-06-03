namespace Splunk.Client
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;

    public interface ITransmitter
    {
        Task SendAsync(Stream eventStream, string indexName = null, TransmitterArgs args = null);
        Task<SearchResult> SendAsync(string eventText, string indexName = null, TransmitterArgs args = null);
    }

    public abstract class ITransmitterContract : ITransmitter
    {
        public Task SendAsync(Stream eventStream, string indexName = null, TransmitterArgs args = null)
        {
            Contract.Requires<ArgumentNullException>(eventStream != null);
            return default(Task);
        }

        public Task<SearchResult> SendAsync(string eventText, string indexName = null, TransmitterArgs args = null)
        {
            Contract.Requires<ArgumentNullException>(eventText != null);
            return default(Task<SearchResult>);
        }
    }

}
