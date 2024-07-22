using System.Reflection;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace ExtracaoCompiladorExecucao.Compiler
{
    public class Compiler
    {
        private static readonly string[] WhitelistedAssemblies =
        {
            "netstandard",
            "mscorlib",
            "System",
            "System.Core",
            "System.Private.CoreLib",
            "System.Runtime",
            "System.Collections",
            "System.Collections.Generic",
            "System.Linq",
            "System.Text",
            "System.Text.Json"
        };

        private readonly CSharpCompilationOptions compilationOptions;
        private readonly CSharpParseOptions parseOptions;

        private readonly object referencesLocker = new object();
        private Assembly[] assemblyReferences;
        private MetadataReference[] metadataReferences;

        public Compiler()
        {
            compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            parseOptions = new CSharpParseOptions(LanguageVersion.Latest, kind: SourceCodeKind.Script);
        }

        public async Task<CompilationResult> CompileAsync(string sourceCode, CancellationToken cancellationToken = default)
        {
            var result = await Task.Run(() =>
            {
                // load references
                lock (referencesLocker)
                {
                    assemblyReferences = GetReferences().ToArray();
                    metadataReferences = assemblyReferences.Select(a => MetadataReference.CreateFromFile(a.Location)).ToArray();
                }

                cancellationToken.ThrowIfCancellationRequested();

                // compile the code
                return CompileSourceCode(SourceText.From(sourceCode), cancellationToken);
            }, cancellationToken);

            if (result.HasErrors)
            {
                foreach (var message in result.CompilationMessages)
                {
                    Console.WriteLine(message.Message);
                }
            }
            return result;
        }

        private IEnumerable<Assembly> GetReferences()
        {
            var _ = JsonSerializer.Serialize("Forçar carregamento de System.Text.Json");

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            return assemblies
                .Where(a => WhitelistedAssemblies.Any(wl => wl.Equals(a.GetName().Name, StringComparison.OrdinalIgnoreCase)));
        }
        private CompilationResult CompileSourceCode(SourceText sourceCode, CancellationToken cancellationToken = default)
        {
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(
                sourceCode, parseOptions, cancellationToken: cancellationToken);

            var compilation = CSharpCompilation.CreateScriptCompilation(
                "ConsoleAppExecRuntime", syntaxTree, metadataReferences, compilationOptions);

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms, cancellationToken: cancellationToken);

                Type scriptType = null;
                if (result.Success)
                {
                    var assembly = Assembly.Load(ms.ToArray());
                    scriptType = assembly.GetType("Script");
                }

                return new CompilationResult
                {
                    CompilationMessages = GetCompilationMessages(result.Diagnostics),
                    ScriptType = scriptType,
                };
            }
        }

        private IEnumerable<CompilationMessage> GetCompilationMessages(IEnumerable<Diagnostic> diagnostics, CancellationToken cancellationToken = default)
        {
            diagnostics = diagnostics
                .Where(d => d.Location.IsInSource)
                .OrderBy(d => d.Severity)
                .OrderBy(d => d.Location.SourceSpan.Start);

            foreach (var diag in diagnostics)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return new CompilationMessage
                {
                    Severity = diag.Severity.ToCompilationMessageSeverity(),
                    Message = $"{diag.Id}: {diag.GetMessage()}",
                    StartOffset = diag.Location.SourceSpan.Start,
                    EndOffset = diag.Location.SourceSpan.End,
                    LineNumber = diag.Location.GetMappedLineSpan().Span.Start.Line + 1
                };
            }
        }
    }
}


