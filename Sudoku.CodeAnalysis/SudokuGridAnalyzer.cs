using Microsoft.CodeAnalysis;

namespace Sudoku.CodeAnalysis
{
	/// <summary>
	/// Indicates the analyzer that checks the usage of the type <c>SudokuGrid</c>.
	/// </summary>
	/// <remarks>
	/// All supported diagnostics:
	/// <list type="bullet">
	/// <item><a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3614979&amp;doc_id=633030">SUDOKU014</a> (The member can't be invoked because they are reserved)</item>
	/// </list>
	/// </remarks>
	[Generator]
	public sealed partial class SudokuGridAnalyzer : ISourceGenerator
	{
		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			var compilation = context.Compilation;
			foreach (var syntaxTree in compilation.SyntaxTrees)
			{
				// Check whether the syntax contains the root node.
				if (!syntaxTree.TryGetRoot(out var root))
				{
					continue;
				}

				var collector = new InnerWalker();
				collector.Visit(root);

				// If the syntax tree doesn't contain any dynamically called clause,
				// just skip it.
				if (collector.Collection is null)
				{
					continue;
				}

				// Iterate on each dynamically called location.
				foreach (var (node, fieldName) in collector.Collection)
				{
					// You can't invoke them.
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: new(
								id: DiagnosticIds.Sudoku014,
								title: Titles.Sudoku014,
								messageFormat: Messages.Sudoku014,
								category: Categories.Usage,
								defaultSeverity: DiagnosticSeverity.Error,
								isEnabledByDefault: true,
								helpLinkUri: HelpLinks.Sudoku014
							),
							location: node.GetLocation(),
							messageArgs: new[] { fieldName }
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
