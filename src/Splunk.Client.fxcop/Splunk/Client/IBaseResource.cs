namespace Splunk.Client
{
    using System;

    public interface IBaseResource
    {
        Version GeneratorVersion { get; }
        Uri Id { get; }
        string Title { get; }
        DateTime Updated { get; }
    }
}
