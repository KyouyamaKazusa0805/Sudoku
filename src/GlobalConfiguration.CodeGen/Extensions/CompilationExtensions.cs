namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides extension methods on <see cref="Compilation"/>.
/// </summary>
/// <seealso cref="Compilation"/>
internal static class CompilationExtensions
{
	/// <summary>
	/// Try to get the <see cref="INamedTypeSymbol"/> result via the specified type instance
	/// specified as the type argument.
	/// </summary>
	/// <typeparam name="TNotNull">
	/// The type to get its corresponding <see cref="INamedTypeSymbol"/> instance as the return value.
	/// Due to the design of Roslyn, the type should be <see langword="notnull"/>, which means you cannot
	/// append nullable notation <c>?</c> onto the type, such as <see cref="int"/>? or <see cref="object"/>?
	/// won't be compiled.
	/// </typeparam>
	/// <param name="this">The compilation instance.</param>
	/// <returns>The corresponding <see cref="INamedTypeSymbol"/> result.</returns>
	/// <seealso cref="INamedTypeSymbol"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static INamedTypeSymbol GetTypeSymbol<TNotNull>(this Compilation @this) where TNotNull : notnull =>
		@this.GetTypeByMetadataName(typeof(TNotNull).FullName)!;
}
