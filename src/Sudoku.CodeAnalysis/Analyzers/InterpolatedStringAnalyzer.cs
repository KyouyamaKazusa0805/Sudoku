using Microsoft.CodeAnalysis;

namespace Sudoku.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates the analyzer that analyzes the interpolated strings.
	/// </summary>
	/// <remarks>
	/// All supported diagnostics:
	/// <list type="bullet">
	/// <item><a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3622115&amp;doc_id=633030">SUDOKU016</a> (Please add 'ToString' method invocation to the interpolation part in order to prevent any box and unbox operations)</item>
	/// <item><a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3629641&amp;doc_id=633030">SUDOKU020</a> (Unnecessary interpolation leading character '$')</item>
	/// </list>
	/// </remarks>
	[Generator]
	public sealed partial class InterpolatedStringAnalyzer : ISourceGenerator
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

				VerifySudoku016(context, root, semanticModel);
				VerifySudoku020(context, root);
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}


		partial void VerifySudoku016(GeneratorExecutionContext context, SyntaxNode root, SemanticModel model);
		partial void VerifySudoku020(GeneratorExecutionContext context, SyntaxNode root);
	}
}
