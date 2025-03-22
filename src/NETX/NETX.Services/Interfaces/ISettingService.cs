using NETX.Core.Models;

namespace NETX.Services.Interfaces
{
    public interface ISettingService : IBaseService<Setting>
    {
        Setting? GetByKey (string key);

        bool ContainsKey(string key);

        Setting Add(string key, string value);

        Task<Setting> AddAsync(string key, string value);

        Setting Update(string key, string value);

        Task<Setting> UpdateAsync(string key, string value);

        void RemoveByKey(string key);

        Task RemoveByKeyAsync(string key);
    }
}
