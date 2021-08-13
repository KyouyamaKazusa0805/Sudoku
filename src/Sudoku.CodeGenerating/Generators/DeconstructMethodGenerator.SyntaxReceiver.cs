namespace Sudoku.CodeGenerating.Generators;

partial class DeconstructMethodGenerator
{
	/// <summary>
	/// Defines the syntax receiver.
	/// </summary>
	private sealed class SyntaxReceiver : ISyntaxReceiver
	{
		/// <summary>
		/// Indicates all possible candidate types used.
		/// </summary>
		public IList<TypeDeclarationSyntax> Candidates { get; } = new List<TypeDeclarationSyntax>();


		/// <inheritdoc/>
		public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
		{
			if (syntaxNode is TypeDeclarationSyntax { AttributeLists.Count: not 0 } declaration)
			{
				Candidates.Add(declaration);
			}
		}
	}
}
