namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides extension methods on <see cref="SyntaxReference"/>.
/// </summary>
/// <seealso cref="SyntaxReference"/>
internal static class SyntaxReferenceExtensions
{
	/// <summary>
	/// Creates a <see cref="Location"/> instance as the result value
	/// via the specified <see cref="SyntaxReference"/>.
	/// </summary>
	/// <param name="this">The instance.</param>
	/// <returns>The <see cref="Location"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Location ToLocation(this SyntaxReference @this) =>
		Location.Create(@this.SyntaxTree, @this.Span);
}
