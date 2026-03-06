using FileRenameTool.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileRenameTool.Services
{
    public class FileProcessingService
    {
        private readonly RuleEngineService _ruleEngine;
        private readonly RuleCacheService _ruleCache;

        public FileProcessingService(RuleEngineService ruleEngine, RuleCacheService ruleCache)
        {
            _ruleEngine = ruleEngine;
            _ruleCache = ruleCache;
        }

        public async Task<List<FileItemModel>> ScanDirectoryAsync(string srcDirectoryPath, string rulePattern, string searchPattern = "*", bool recursive = false, string? fileNameRegex = null)
        {
            var results = new List<FileItemModel>();
            if (!Directory.Exists(srcDirectoryPath))
                return results;

            // 預先編譯 Pattern 提升批次效能，並加入快取機制
            var compiledFunc = _ruleCache.GetOrAdd(
                rulePattern,
                pattern => _ruleEngine.CompileRulePattern(pattern));

            var options = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.GetFiles(srcDirectoryPath, searchPattern, options);
            
            Regex? regex = null;
            if (!string.IsNullOrWhiteSpace(fileNameRegex))
            {
                try
                {
                    regex = new Regex(fileNameRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                }
                catch
                {
                    // Ignore invalid regex
                }
            }

            foreach (var file in files)
            {
                var model = new FileItemModel(file, srcDirectoryPath);
                
                if (regex != null && !regex.IsMatch(model.OriginalFileName))
                {
                    continue; // Skip files that do not match the regex
                }
                try
                {
                    var evaluated = await compiledFunc(model.ToContext());
                    model.NewFileName = Regex.Replace(evaluated, @"\\{2,}", "\\");  // 將連續兩個或以上的反斜線合併為單一反斜線
                }
                catch (System.Exception ex)
                {
                    model.Status = "Error";
                    model.ErrorMessage = ex.Message;
                }
                results.Add(model);
            }
            return results;
        }

        public void ExecuteRename(List<FileItemModel> items, string destDirectoryPath)
        {
            if (!Directory.Exists(destDirectoryPath))
                Directory.CreateDirectory(destDirectoryPath);

            foreach (var item in items)
            {
                if (item.Status == "Error" || string.IsNullOrWhiteSpace(item.NewFileName))
                    continue;

                try
                {
                    // To handle path separators from evaluated result (e.g. {yyyyMMdd}/{old_FileName})
                    string destPath = Path.Combine(destDirectoryPath, item.NewFileName);
                    
                    // Ensure the sub-directory exists if user used path separators like "/"
                    string? destDir = Path.GetDirectoryName(destPath);
                    if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                    {
                        Directory.CreateDirectory(destDir);
                    }

                    // Prevent overwrite by adding index
                    int counter = 1;
                    string fileNameOnly = Path.GetFileNameWithoutExtension(destPath);
                    string extension = Path.GetExtension(destPath);
                    string originalDestPath = destPath;

                    while (File.Exists(destPath))
                    {
                        destPath = Path.Combine(destDir ?? "", $"{fileNameOnly}_{counter++}{extension}");
                    }

                    File.Move(item.OriginalPath, destPath);
                    
                    item.Status = "Success";
                    item.OriginalPath = destPath;
                    item.OriginalFileName = Path.GetFileName(destPath);
                }
                catch (System.Exception ex)
                {
                    item.Status = "Error";
                    item.ErrorMessage = ex.Message;
                }
            }
        }
    }
}
