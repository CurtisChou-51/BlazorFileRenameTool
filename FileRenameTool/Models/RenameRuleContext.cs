using System;

namespace FileRenameTool.Models
{
    public class RenameRuleContext
    {
        public string FileName { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public DateTime CreationTime { get; set; }
        public DateTime LastWriteTime { get; set; }

        // Provide shortcut format for common use cases
        public string yyyyMMdd => CreationTime.ToString("yyyyMMdd");
        public string yyyyMM => CreationTime.ToString("yyyyMM");

        // The relative path from the scanned root directory
        public string RelativePath { get; set; } = string.Empty;
    }
}
