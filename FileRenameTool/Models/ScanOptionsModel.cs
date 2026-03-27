namespace FileRenameTool.Models
{
    public class ScanOptionsModel
    {
        public string SrcDirectoryPath { get; set; } = string.Empty;
        public string RulePattern { get; set; } = string.Empty;
        public string SearchPattern { get; set; } = "*";
        public bool IsRecursive { get; set; } = false;
        public string? FileNameRegex { get; set; }
    }
}
