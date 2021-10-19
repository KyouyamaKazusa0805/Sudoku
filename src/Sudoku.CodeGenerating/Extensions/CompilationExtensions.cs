namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides extension methods on <see cref="Compilation"/>.
/// </summary>
/// <seealso cref="Compilation"/>
internal static class CompilationExtensions
{
	/// <summary>
	/// <para>
	/// Check all type arguments recursively whether any types are marked specified attribute type.
	/// </para>
	/// <para>
	/// This method will check all type arguments for this type symbol.
	/// For example, if the type symbol is like this:
	/// <code><![CDATA[IReadOnlyDictionary<string, IReadOnlyList<IReadOnlyList<int>>>]]></code>
	/// this method will check those parts:
	/// <list type="bullet">
	/// <item><c><![CDATA[IReadOnlyDictionary<string, IReadOnlyList<IReadOnlyList<int>>>]]></c></item>
	/// <item><c>string</c></item>
	/// <item><c><![CDATA[IReadOnlyList<IReadOnlyList<int>>]]></c></item>
	/// <item><c><![CDATA[IReadOnlyList<int>]]></c></item>
	/// <item><c>int</c></item>
	/// </list>
	/// </para>
	/// </summary>
	/// <typeparam name="TAttribute">The type of the attribute to check.</typeparam>
	/// <param name="this">The compilation.</param>
	/// <param name="symbol">The symbol to check.</param>
	/// <returns>
	/// A <see cref="bool"/> result indicating:
	/// <list type="table">
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>
	/// The type symbol contains a argument that is marked <typeparamref name="TAttribute"/>.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>All type arguments aren't marked <typeparamref name="TAttribute"/>.</description>
	/// </item>
	/// </list>
	/// </returns>
	public static bool TypeArgumentMarked<TAttribute>(this Compilation @this, INamedTypeSymbol symbol)
	where TAttribute : Attribute
	{
		var attribute = @this.GetTypeByMetadataName(typeof(TAttribute).FullName)!;

		return f(symbol);


		bool f(INamedTypeSymbol? symbol) => symbol switch
		{
			null => false,
			{ IsGenericType: false } => symbol.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)),
			{ TypeArguments: var typeArgs } => typeArgs.All(t => f(t as INamedTypeSymbol))
		};
	}
}
