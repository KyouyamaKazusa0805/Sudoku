namespace Sudoku.Diagnostics.CodeGen;

partial class SyntaxContextReceiverCreator
{
	/// <summary>
	/// Indicates the default syntax context receiver.
	/// </summary>
	/// <param name="SyntaxNodeVisitor">
	/// The syntax node visitor method that determines
	/// whether the syntax node can be used by a source generator.
	/// </param>
	private sealed record DefaultReceiver(Action<SyntaxNode, SemanticModel> SyntaxNodeVisitor) : ISyntaxContextReceiver
	{
		/// <inheritdoc/>
		public void OnVisitSyntaxNode(GeneratorSyntaxContext context) =>
			SyntaxNodeVisitor(context.Node, context.SemanticModel);
	}
}
