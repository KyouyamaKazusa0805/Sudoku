namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// Encapsulates a set of methods to create the <see cref="ISyntaxContextReceiver"/>s.
/// </summary>
/// <seealso cref="ISyntaxContextReceiver"/>
public static partial class SyntaxContextReceiverCreator
{
	/// <summary>
	/// Creates a <see cref="ISyntaxContextReceiver"/> instance to determine whether the syntax node satisfies
	/// some condition and can be used by the source generator.
	/// </summary>
	/// <param name="syntaxNodeVisitor">The syntax node visitor method.</param>
	/// <returns>The <see cref="ISyntaxContextReceiver"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ISyntaxContextReceiver Create(Action<SyntaxNode, SemanticModel> syntaxNodeVisitor) =>
		new DefaultReceiver(syntaxNodeVisitor);
}
