using FileRenameTool.Models;
using System.Text.Json;

namespace FileRenameTool.Services
{
    public class ConfigStorageService
    {
        private readonly string _filePath = Path.Combine(AppContext.BaseDirectory, "saved_configs.json");
        private readonly static JsonSerializerOptions _options = new() { WriteIndented = true };

        public async Task<Dictionary<string, RenameConfigModel>> LoadConfigsAsync()
        {
            if (!File.Exists(_filePath))
                return [];

            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<Dictionary<string, RenameConfigModel>>(json) ?? [];
        }

        public async Task SaveConfigAsync(string name, RenameConfigModel config)
        {
            var configs = await LoadConfigsAsync();
            configs[name] = config;
            var json = JsonSerializer.Serialize(configs, _options);
            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task DeleteConfigAsync(string name)
        {
            var configs = await LoadConfigsAsync();
            if (configs.Remove(name))
            {
                var json = JsonSerializer.Serialize(configs, _options);
                await File.WriteAllTextAsync(_filePath, json);
            }
        }
    }
}
