using Microsoft.CodeAnalysis;

namespace Sudoku.CodeAnalysis
{
	/// <summary>
	/// Indicates the analyzer that analyzes on types <c>Cells</c> and <c>Candidates</c>.
	/// </summary>
	/// <remarks>
	/// All supported diagnostics:
	/// <list type="bullet">
	/// <item><a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3625575&amp;doc_id=633030">SUDOKU018</a> (Replace 'Count == 0' with 'IsEmpty')</item>
	/// </list>
	/// </remarks>
	[Generator]
	public sealed partial class CellsOrCandidatesAnalyzer : ISourceGenerator
	{
		/// <summary>
		/// Indicates the zero string.
		/// </summary>
		private const string ZeroString = "0";

		/// <summary>
		/// Indicates the property name of <c>Count</c>.
		/// </summary>
		private const string CountPropertyName = "Count";

		/// <summary>
		/// Indicates the full type name of <c>Cells</c>.
		/// </summary>
		private const string CellsFullTypeName = "Sudoku.Data.Cells";

		/// <summary>
		/// Indicates the full type name of <c>Candidates</c>.
		/// </summary>
		private const string CandidatesFullTypeName = "Sudoku.Data.Candidates";


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

				// Create the semantic model and the property list.
				var semanticModel = compilation.GetSemanticModel(syntaxTree);
				var collector = new InnerWalker(semanticModel, compilation);
				collector.Visit(root);

				// If the syntax tree doesn't contain any dynamically called clause,
				// just skip it.
				if (collector.Collection is null)
				{
					continue;
				}

				// Iterate on each location.
				foreach (var (expr, eqToken, node) in collector.Collection)
				{
					// No calling conversion.
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: new(
								id: DiagnosticIds.Sudoku018,
								title: Titles.Sudoku018,
								messageFormat: Messages.Sudoku018,
								category: Categories.Usage,
								defaultSeverity: DiagnosticSeverity.Warning,
								isEnabledByDefault: true,
								helpLinkUri: HelpLinks.Sudoku018
							),
							location: node.GetLocation(),
							messageArgs: new[] { expr, eqToken, eqToken == "==" ? string.Empty : "!" }
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
