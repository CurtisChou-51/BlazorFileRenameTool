using System;

namespace FileRenameTool.Models
{
    public class RenameRuleContext
    {
        public string FileName { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public DateTime CreationTime { get; set; }
        public DateTime LastWriteTime { get; set; }
        public string RelativePath { get; set; } = string.Empty;

        // Provide shortcut format for common use cases
        public string Creation_yyyyMMdd => CreationTime.ToString("yyyyMMdd");
        public string Creation_yyyyMM => CreationTime.ToString("yyyyMM");
        public string LastWrite_yyyyMMdd => LastWriteTime.ToString("yyyyMMdd");
        public string LastWrite_yyyyMM => LastWriteTime.ToString("yyyyMM");
    }
}
