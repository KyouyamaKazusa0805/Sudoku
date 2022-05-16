namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// Provides with a set of template methods that is used for <see cref="IIncrementalGenerator"/> instances.
/// </summary>
/// <seealso cref="IIncrementalGenerator"/>
internal static class TemplateMethods
{
	/// <summary>
	/// Determines whether a <see cref="SyntaxNode"/> is a <see cref="TypeDeclarationSyntax"/>,
	/// and contains at least one attribute marked onto it.
	/// </summary>
	/// <param name="node">The node to be verified.</param>
	/// <param name="_">
	/// The cancellation token that can cancel the operation.
	/// This argument won't be used in this method; in other words, a discard.
	/// </param>
	/// <returns>A <see cref="bool"/> value.</returns>
	public static bool NodePredicate(SyntaxNode node, CancellationToken _)
		=> node is TypeDeclarationSyntax { Modifiers: var modifiers, AttributeLists: not [] }
			&& modifiers.Any(SyntaxKind.PartialKeyword);
}
