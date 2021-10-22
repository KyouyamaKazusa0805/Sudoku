#undef SUPPORT_RECORD_TYPE_SYNTAX_CONTEXT_RECEIVER

namespace Sudoku.Diagnostics.CodeGen;

partial class SyntaxContextReceiverCreator
{
#if SUPPORT_RECORD_TYPE_SYNTAX_CONTEXT_RECEIVER
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
#else
	/// <summary>
	/// Indicates the default syntax context receiver.
	/// </summary>
	private sealed class DefaultReceiver : ISyntaxContextReceiver
	{
		/// <summary>
		/// Initializes a <see cref="DefaultReceiver"/> instance via the syntax node visitor.
		/// </summary>
		/// <param name="syntaxNodeVisitor">The syntax node visitor.</param>
		public DefaultReceiver(Action<SyntaxNode, SemanticModel> syntaxNodeVisitor) =>
			SyntaxNodeVisitor = syntaxNodeVisitor;


		/// <summary>
		/// The syntax node visitor method that determines
		/// whether the syntax node can be used by a source generator.
		/// </summary>
		public Action<SyntaxNode, SemanticModel> SyntaxNodeVisitor { get; }


		/// <inheritdoc/>
		public void OnVisitSyntaxNode(GeneratorSyntaxContext context) =>
			SyntaxNodeVisitor(context.Node, context.SemanticModel);
	}
#endif
}
