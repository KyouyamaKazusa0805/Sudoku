namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides extension methods on member existence checking.
/// </summary>
internal static class MemberExistence
{
	/// <summary>
	/// Determines whether the member symbol collection contains a specified field.
	/// </summary>
	/// <param name="this">The collection.</param>
	/// <param name="predicate">The condition to satisfy.</param>
	/// <param name="result">The found result.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool ExistField(this ImmutableArray<ISymbol> @this, Func<IFieldSymbol, bool> predicate, out IFieldSymbol? result)
	{
		result = @this.OfType<IFieldSymbol>().FirstOrDefault(predicate);
		return result is not null;
	}

	/// <summary>
	/// Determines whether the member symbol collection contains a specified property.
	/// </summary>
	/// <param name="this">The collection.</param>
	/// <param name="predicate">The condition to satisfy.</param>
	/// <param name="result">The found result.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool ExistProperty(this ImmutableArray<ISymbol> @this, Func<IPropertySymbol, bool> predicate, out IPropertySymbol? result)
	{
		result = @this.OfType<IPropertySymbol>().FirstOrDefault(predicate);
		return result is not null;
	}
}
