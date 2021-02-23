using Microsoft.CodeAnalysis;

namespace Sudoku.CodeAnalysis
{
	/// <summary>
	/// Indicates the analyzer that checks the usage of the type <c>SudokuGrid</c>.
	/// </summary>
	[Generator]
	public sealed partial class SudokuGridAnalyzer : ISourceGenerator
	{
		/// <summary>
		/// Indicates the type name of the sudoku grid.
		/// </summary>
		private const string SudokuGridTypeName = "SudokuGrid";

		/// <summary>
		/// Indicates the field name "<c>RefreshingCandidates</c>".
		/// </summary>
		private const string RefreshingCandidatesFuncPtrName = "RefreshingCandidates";

		/// <summary>
		/// Indicates the field name "<c>ValueChanged</c>".
		/// </summary>
		private const string ValueChangedFuncPtrName = "ValueChanged";


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

				var collector = new SudokuGridFuncPtrInvocationSearcher();
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
