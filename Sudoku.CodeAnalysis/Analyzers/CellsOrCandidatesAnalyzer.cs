using Microsoft.CodeAnalysis;

namespace Sudoku.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates the analyzer that analyzes on types <c>Cells</c> and <c>Candidates</c>.
	/// </summary>
	/// <remarks>
	/// All supported diagnostics:
	/// <list type="bullet">
	/// <item><a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3625575&amp;doc_id=633030">SUDOKU018</a> (Replace <c>Count == 0</c> with <c>IsEmpty</c>)</item>
	/// <item><a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3630107&amp;doc_id=633030">SUDOKU021</a> (Please use the field <c>Empty</c> to avoid instantiation)</item>
	/// </list>
	/// </remarks>
	[Generator]
	public sealed partial class CellsOrCandidatesAnalyzer : ISourceGenerator
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

				// Create the semantic model and the property list.
				var semanticModel = compilation.GetSemanticModel(syntaxTree);
				CheckSudoku018(context, root, compilation, semanticModel);
				CheckSudoku021(context, root, compilation, semanticModel);
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}


		partial void CheckSudoku018(GeneratorExecutionContext context, SyntaxNode root, Compilation compilation, SemanticModel semanticModel);
		partial void CheckSudoku021(GeneratorExecutionContext context, SyntaxNode root, Compilation compilation, SemanticModel semanticModel);
	}
}
