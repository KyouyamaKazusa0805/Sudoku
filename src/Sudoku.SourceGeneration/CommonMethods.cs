namespace Sudoku.SourceGeneration;

/// <summary>
/// Represents a set of methods that can be used by the types in this file.
/// </summary>
internal static class CommonMethods
{
	/// <summary>
	/// Determine whether the specified <see cref="SyntaxNode"/> is of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The possible type of the node.</typeparam>
	/// <param name="node">Indicates the target node.</param>
	/// <param name="_"/>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool SyntaxNodeTypePredicate<T>(SyntaxNode node, CancellationToken _) where T : SyntaxNode => node is T;

	/// <summary>
	/// Determine whether the specified type declaration syntax node contains a <see langword="partial"/> modifier.
	/// </summary>
	/// <typeparam name="T">The type of the declaration syntax node.</typeparam>
	/// <param name="node">The node to be determined.</param>
	/// <param name="_"/>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool IsPartialTypePredicate<T>(T node, CancellationToken _) where T : SyntaxNode
		=> node is BaseTypeDeclarationSyntax { Modifiers: var modifiers and not [] } && modifiers.Any(SyntaxKind.PartialKeyword);

	/// <summary>
	/// Determine whether the specified <see cref="SyntaxNode"/> is a <see cref="MethodDeclarationSyntax"/>,
	/// and contains <see langword="partial"/> modifier.
	/// </summary>
	/// <param name="node">The node.</param>
	/// <param name="_"/>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool IsPartialMethodPredicate(SyntaxNode node, CancellationToken _)
		=> node is MethodDeclarationSyntax { Modifiers: var m } && m.Any(SyntaxKind.PartialKeyword);

	/// <summary>
	/// Determine whether the value is not <see langword="null"/>.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="value">The value.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool NotNullPredicate<T>(T value) => value is not null;

	/// <summary>
	/// Try to get the internal value without nullability checking.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="value">The value with <c>?</c> token being annotated, but not <see langword="null"/> currently.</param>
	/// <param name="_"/>
	/// <returns>The value.</returns>
	public static T NotNullSelector<T>(T? value, CancellationToken _) where T : class => value!;

	/// <summary>
	/// Try to get the internal value without nullability checking.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="value">The value with <c>?</c> token being annotated, but not <see langword="null"/> currently.</param>
	/// <param name="_"/>
	/// <returns>The value.</returns>
	public static T NotNullSelector<T>(T? value, CancellationToken _) where T : struct => value!.Value;
}
