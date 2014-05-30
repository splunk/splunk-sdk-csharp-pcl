using System;
namespace Splunk.Client.Refactored
{
    interface ISavedSearchCollection
    {
        System.Threading.Tasks.Task<SavedSearch> CreateAsync(string name, string search, Splunk.Client.SavedSearchAttributes attributes, Splunk.Client.SavedSearchDispatchArgs dispatchArgs = null, Splunk.Client.SavedSearchTemplateArgs templateArgs = null);
        System.Collections.Generic.IReadOnlyList<Splunk.Client.Message> Messages { get; }
        Splunk.Client.Pagination Pagination { get; }
    }
}
