using System;
using System.IO;

namespace FileRenameTool.Models
{
    public class FileItemModel
    {
        public string OriginalPath { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string NewFileName { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string ErrorMessage { get; set; } = string.Empty;
        
        // Metadata
        public string Extension { get; set; } = string.Empty;
        public DateTime CreationTime { get; set; }
        public DateTime LastWriteTime { get; set; }

        public string BaseDirectory { get; set; } = string.Empty;
        public string RelativePath { get; set; } = string.Empty;

        public FileItemModel(string filePath, string baseDirectory)
        {
            var fileInfo = new FileInfo(filePath);
            OriginalPath = filePath;
            OriginalFileName = fileInfo.Name;
            Extension = fileInfo.Extension;
            CreationTime = fileInfo.CreationTime;
            LastWriteTime = fileInfo.LastWriteTime;

            BaseDirectory = baseDirectory;
            if (filePath.StartsWith(baseDirectory, StringComparison.OrdinalIgnoreCase))
            {
                RelativePath = filePath.Substring(baseDirectory.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                RelativePath = Path.GetDirectoryName(RelativePath) ?? string.Empty; // 取得相對於 BaseDirectory 的資料夾路徑，不包含檔名
            }
        }

        public RenameRuleContext ToContext()
        {
            return new RenameRuleContext
            {
                FileName = Path.GetFileNameWithoutExtension(OriginalFileName),
                Extension = Extension,
                CreationTime = CreationTime,
                LastWriteTime = LastWriteTime,
                RelativePath = RelativePath
            };
        }
    }
}
