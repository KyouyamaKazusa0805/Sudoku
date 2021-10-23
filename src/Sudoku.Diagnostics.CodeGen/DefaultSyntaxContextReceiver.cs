namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// Indicates the default syntax context receiver.
/// </summary>
/// <param name="SyntaxNodeVisitor">
/// The syntax node visitor method that determines
/// whether the syntax node can be used by a source generator.
/// </param>
internal sealed record DefaultSyntaxContextReceiver(Action<SyntaxNode, SemanticModel> SyntaxNodeVisitor) : ISyntaxContextReceiver
{
	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context) =>
		SyntaxNodeVisitor(context.Node, context.SemanticModel);
}