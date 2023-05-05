﻿namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides with extension methods on <see cref="ISymbol"/>.
/// </summary>
internal static class ISymbolExtensions
{
	/// <summary>
	/// Determines whether the specified <see cref="ISymbol"/> instance has marked the specified attribute type.
	/// </summary>
	/// <typeparam name="T">The type of the symbol.</typeparam>
	/// <param name="this">The instance.</param>
	/// <param name="attributeType">The attribute type.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ContainsAttribute<T>(this T @this, INamedTypeSymbol attributeType) where T : ISymbol
		=> @this.GetAttributes().Any(e => SymbolEqualityComparer.Default.Equals(e.AttributeClass, attributeType));
}
