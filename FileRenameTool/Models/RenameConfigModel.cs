using System.Text.Json.Serialization;

namespace FileRenameTool.Models
{
    public class RenameConfigModel
    {
        public string SrcDirectoryPath { get; set; } = string.Empty;
        public string DestDirectoryPath { get; set; } = string.Empty;
        public string SearchPattern { get; set; } = "*";
        public bool IsRecursive { get; set; } = false;
        public string FileNameRegex { get; set; } = string.Empty;
        public List<string> RuleSegments { get; set; } = [];
        
        [JsonIgnore]
        public string RulePattern => string.Join("\\", RuleSegments.Where(s => !string.IsNullOrWhiteSpace(s)));
    }
}
