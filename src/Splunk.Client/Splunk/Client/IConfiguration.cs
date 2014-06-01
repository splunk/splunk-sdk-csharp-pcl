namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IConfiguration<TConfigurationStanza> : IEntityCollection<TConfigurationStanza>
        where TConfigurationStanza : BaseEntity, IConfigurationStanza, new()
    {
        Task<TConfigurationStanza> CreateAsync(IEnumerable<Argument> arguments);
        Task<TConfigurationStanza> CreateAsync(string stanzaName);
        Task<string> GetSettingAsync(string stanzaName, string keyName);
        Task RemoveAsync(string stanzaName);
        Task<TConfigurationStanza> UpdateAsync(string stanzaName, params Argument[] settings);
        Task UpdateSettingAsync(string stanzaName, string keyName, string value);
    }
}
