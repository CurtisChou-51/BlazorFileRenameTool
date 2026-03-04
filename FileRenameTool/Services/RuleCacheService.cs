using FileRenameTool.Models;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace FileRenameTool.Services
{
    public class RuleCacheService
    {
        private readonly ConcurrentDictionary<string, Func<RenameRuleContext, Task<string>>> _compiledFuncCache = new();

        public Func<RenameRuleContext, Task<string>> GetOrAdd(string rulePattern, Func<string, Func<RenameRuleContext, Task<string>>> valueFactory)
        {
            return _compiledFuncCache.GetOrAdd(rulePattern, valueFactory);
        }
    }
}
