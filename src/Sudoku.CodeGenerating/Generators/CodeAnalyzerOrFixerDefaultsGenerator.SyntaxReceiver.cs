namespace Sudoku.CodeGenerating.Generators;

partial class CodeAnalyzerOrFixerDefaultsGenerator
{
	/// <summary>
	/// Defines a syntax receiver.
	/// </summary>
	private sealed class SyntaxReceiver : ISyntaxReceiver
	{
		/// <summary>
		/// Indicates all possible candidate types used.
		/// </summary>
		public IList<ClassDeclarationSyntax> Candidates { get; } = new List<ClassDeclarationSyntax>();


		/// <inheritdoc/>
		public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
		{
			if (syntaxNode is ClassDeclarationSyntax { AttributeLists.Count: not 0 } declaration)
			{
				Candidates.Add(declaration);
			}
		}
	}
}
