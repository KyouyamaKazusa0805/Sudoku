namespace Sudoku.CodeGenerating.Generators;

partial class PrivatizeParameterlessConstructorGenerator
{
	/// <summary>
	/// Defines the syntax receiver.
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
			// Any field with at least one attribute is a candidate for property generation.
			if (syntaxNode is ClassDeclarationSyntax { AttributeLists.Count: not 0 } classDeclaration)
			{
				Candidates.Add(classDeclaration);
			}
		}
	}
}
