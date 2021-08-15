namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides extension methods on <see cref="Compilation"/>.
/// </summary>
/// <seealso cref="Compilation"/>
internal static class CompilationExtensions
{
	/// <inheritdoc cref="Compilation.GetTypeByMetadataName(string)"/>
	/// <typeparam name="T">The type of the metadata type name to construct.</typeparam>
	/// <param name="this">The compilation.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static INamedTypeSymbol? GetTypeByMetadataName<T>(this Compilation @this) =>
		@this.GetTypeByMetadataName(typeof(T).FullName);
}
