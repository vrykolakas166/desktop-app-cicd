using NETX.Core;
using NETX.Core.Models;
using NETX.Services.Interfaces;

namespace NETX.Services
{
    public class SettingService(NXDbContext context) : BaseService<Setting>(context), ISettingService
    {
        private readonly NXDbContext _context = context;

        public Setting Add(string key, string value)
        {
            if (_context.Settings.Any(s => s.Key == key))
            {
                throw new ArgumentException("Setting with the same key already exists.");
            }

            var setting = new Setting()
            {
                Key = key,
                Value = value
            };

            _context.Settings.Add(setting);
            _context.SaveChanges();
            return setting;
        }

        public async Task<Setting> AddAsync(string key, string value)
        {
            if (_context.Settings.Any(s => s.Key == key))
            {
                throw new ArgumentException("Setting with the same key already exists.");
            }

            var setting = new Setting()
            {
                Key = key,
                Value = value
            };

            await _context.Settings.AddAsync(setting);
            await _context.SaveChangesAsync();
            return setting;
        }

        public Setting Update(string key, string value)
        {
            var setting = _context.Settings.FirstOrDefault(s => s.Key == key) ?? throw new ArgumentException("Setting with the key does not exist.");
            setting.Value = value;
            _context.Settings.Update(setting);
            _context.SaveChanges();
            return setting;
        }

        public async Task<Setting> UpdateAsync(string key, string value)
        {
            var setting = _context.Settings.FirstOrDefault(s => s.Key == key) ?? throw new ArgumentException("Setting with the key does not exist.");
            setting.Value = value;
            _context.Settings.Update(setting);
            await _context.SaveChangesAsync();
            return setting;
        }

        public bool ContainsKey(string key)
        {
            return _context.Settings.Any(setting => setting.Key == key);
        }

        public Setting? GetByKey(string key)
        {
            return _context.Settings.FirstOrDefault(setting => setting.Key == key);
        }

        public void RemoveByKey(string key)
        {
            var setting = GetByKey(key);
            if (setting != null)
            {
                _context.Settings.Remove(setting);
                _context.SaveChanges();
            }
        }

        public async Task RemoveByKeyAsync(string key)
        {
            var setting = GetByKey(key);
            if (setting != null)
            {
                _context.Settings.Remove(setting);
                await _context.SaveChangesAsync();
            }
        }
    }
}
