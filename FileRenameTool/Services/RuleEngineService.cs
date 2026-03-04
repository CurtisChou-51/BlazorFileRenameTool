using FileRenameTool.Models;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Text.RegularExpressions;

namespace FileRenameTool.Services
{
    public class RuleEngineService
    {
        // 匹配括號 { ... } 的正規表達式
        private static readonly Regex PatternRegex = new Regex(@"\{([^}]+)\}", RegexOptions.Compiled);

        public Func<RenameRuleContext, Task<string>> CompileRulePattern(string rawPattern)
        {
            if (string.IsNullOrWhiteSpace(rawPattern))
                return context => Task.FromResult(context.FileName + context.Extension);

            var options = ScriptOptions.Default
                .AddReferences(typeof(RenameRuleContext).Assembly)
                .AddImports("System", "System.Linq");

            var matches = PatternRegex.Matches(rawPattern);
            var compiledExpressions = new List<(Match match, ScriptRunner<object>? runner)>();

            foreach (Match match in matches)
            {
                var expression = match.Groups[1].Value;
                try
                {
                    // 預先編譯並建立委派
                    var script = CSharpScript.Create(expression, options, globalsType: typeof(RenameRuleContext));
                    var runner = script.CreateDelegate();
                    compiledExpressions.Add((match, runner));
                }
                catch (Exception)
                {
                    // 若編譯失敗，在此存入 null runner 以便後續執行時回報錯誤
                    compiledExpressions.Add((match, null));
                }
            }

            return async context =>
            {
                string result = rawPattern;
                foreach (var (match, runner) in compiledExpressions)
                {
                    if (runner == null)
                    {
                        result = result.Replace(match.Value, "[Error: Compile Failed]");
                        continue;
                    }

                    try
                    {
                        var evalResult = await runner(context);
                        result = result.Replace(match.Value, evalResult?.ToString() ?? string.Empty);
                    }
                    catch (Exception ex)
                    {
                        result = result.Replace(match.Value, $"[Error: {ex.Message}]");
                    }
                }
                return result;
            };
        }
    }
}
