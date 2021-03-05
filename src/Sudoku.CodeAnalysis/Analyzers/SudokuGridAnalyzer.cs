using Microsoft.CodeAnalysis;

namespace Sudoku.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates the analyzer that checks the usage of the type <c>SudokuGrid</c>.
	/// </summary>
	/// <remarks>
	/// All supported diagnostics:
	/// <list type="bullet">
	/// <item><a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3614979&amp;doc_id=633030">SUDOKU014</a> (The member can't be invoked because they are reserved)</item>
	/// <item><a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3630107&amp;doc_id=633030">SUDOKU021</a> (Please use the field <c>Undefined</c> to avoid instantiation)</item>
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

				var semanticModel = compilation.GetSemanticModel(syntaxTree);
				CheckSudoku014(context, root);
				CheckSudoku021(context, root, semanticModel, compilation);
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}


		partial void CheckSudoku014(GeneratorExecutionContext context, SyntaxNode root);
		partial void CheckSudoku021(GeneratorExecutionContext context, SyntaxNode root, SemanticModel semanticModel, Compilation compilation);
	}
}
