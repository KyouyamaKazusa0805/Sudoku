namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides with extension methods on <see cref="ISymbol"/>.
/// </summary>
/// <seealso cref="ISymbol"/>
internal static class ISymbolExtensions
{
	/// <summary>
	/// Gets the type of the member specified as the base type <see cref="ISymbol"/>.
	/// </summary>
	/// <param name="this">The type symbol.</param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException">Throws when the argument is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ITypeSymbol GetMemberType(this ISymbol @this)
		=> @this switch
		{
			IFieldSymbol { Type: var type } => type,
			IPropertySymbol { Type: var type } => type,
			IEventSymbol { Type: var type } => type,
			//IMethodSymbol { ReturnType: var type } => type,
			_ => throw new InvalidOperationException("The specified argument is invalid.")
		};
}
