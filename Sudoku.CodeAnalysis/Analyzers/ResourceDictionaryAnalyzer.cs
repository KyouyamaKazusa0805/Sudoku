using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace Sudoku.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates the analyzer that is called the resource dictionary values. Both two resource dictionaries
	/// store in the folder <c>..\required\lang</c>.
	/// </summary>
	/// <remarks>
	/// All supported diagnostics:
	/// <list type="bullet">
	/// <item><a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3608009&amp;doc_id=633030">SUDOKU009</a> (The specified key can't be found in the resource dictionary)</item>
	/// </list>
	/// </remarks>
	[Generator]
	public sealed partial class ResourceDictionaryAnalyzer : ISourceGenerator
	{
		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			var compilation = context.Compilation;
			if (compilation.AssemblyName is "Sudoku.UI" or "Sudoku.Windows")
			{
				// We don't check on those two WPF projects, because those two projects has already used
				// their own resource dictionary (MergedDictionary).
				return;
			}

			// Check all syntax trees if available.
			string[] texts = (from file in context.AdditionalFiles select File.ReadAllText(file.Path)).ToArray();
			if (texts.Length == 0)
			{
				return;
			}

			foreach (var syntaxTree in compilation.SyntaxTrees)
			{
				// Check whether the syntax contains the root node.
				if (!syntaxTree.TryGetRoot(out var root))
				{
					continue;
				}

				// Create the semantic model and the property list.
				var semanticModel = compilation.GetSemanticModel(syntaxTree);
				var collector = new InnerWalker(semanticModel);
				collector.Visit(root);

				// If the syntax tree doesn't contain any dynamically called clause,
				// just skip it.
				if (collector.Collection is null)
				{
					continue;
				}

				// Iterate on each dynamically called location.
				foreach (var (node, value) in collector.Collection)
				{
					var jsonProprtyNameRegex = new Regex($@"""{value}""(?=\:\s""[^""]+"",?)");

					// Check all dictionaries. If all dictionaries don't contain that key,
					// we'll report on this.
					if (texts.Any(text => jsonProprtyNameRegex.Match(text).Success))
					{
						continue;
					}

					// Report the diagnostic result.
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: new(
								id: DiagnosticIds.Sudoku009,
								title: Titles.Sudoku009,
								messageFormat: Messages.Sudoku009,
								category: Categories.ResourceDictionary,
								defaultSeverity: DiagnosticSeverity.Error,
								isEnabledByDefault: true,
								helpLinkUri: HelpLinks.Sudoku009
							),
							location: node.Name.GetLocation(),
							messageArgs: new[] { value }
						)
					);
				}
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}
	}
}
