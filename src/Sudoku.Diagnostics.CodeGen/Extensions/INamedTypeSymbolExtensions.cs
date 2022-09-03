namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides extension methods for <see cref="INamedTypeSymbol"/>.
/// </summary>
/// <seealso cref="INamedTypeSymbol"/>
internal static class INamedTypeSymbolExtensions
{
	/// <summary>
	/// Gets all possible members in a type, even including its base type members.
	/// </summary>
	/// <param name="this">The type symbol.</param>
	/// <returns>All members.</returns>
	public static IEnumerable<ISymbol> GetAllMembers(this INamedTypeSymbol @this)
	{
		for (var current = @this; current is not null; current = current.BaseType)
		{
			foreach (var member in current.GetMembers())
			{
				yield return member;
			}
		}
	}

	/// <summary>
	/// Get the file name of the type symbol.
	/// </summary>
	/// <param name="this">The symbol.</param>
	/// <returns>
	/// The file name. Due to the limited file name and the algorithm, if:
	/// <list type="bullet">
	/// <item>
	/// The character is <c><![CDATA['<']]></c> or <c><![CDATA['>']]></c>:
	/// Change them to <c>'['</c> and <c>']'</c>.
	/// </item>
	/// <item>The character is <c>','</c>: Change it to <c>'_'</c>.</item>
	/// <item>The character is <c>' '</c>: Remove it.</item>
	/// </list>
	/// </returns>
	internal static string ToFileName(this INamedTypeSymbol @this)
	{
		string result = @this.ToDisplayString(ExtendedSymbolDisplayFormat.FullyQualifiedFormatWithConstraints);
		scoped var buffer = (stackalloc char[result.Length]);
		buffer.Fill('\0');
		int pointer = 0;
		for (int i = 0, length = result.Length; i < length; i++)
		{
			switch (result[i])
			{
				case '<': { buffer[pointer++] = '['; break; }
				case '>': { buffer[pointer++] = ']'; break; }
				case ',': { buffer[pointer++] = '_'; break; }
				case ' ' or ':': { continue; }
				default: { buffer[pointer++] = result[i]; break; }
			}
		}

		return buffer[..pointer].ToString();
	}

	/// <summary>
	/// Gets the type kind modifier for a symbol.
	/// </summary>
	/// <param name="this">The named type symbol.</param>
	/// <returns>The string as the representation of the type kind modifier.</returns>
	/// <exception cref="ArgumentException">
	/// Throws when the current named type symbol holds an invalid case that doesn't contain
	/// any possible type kind modifier.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static string GetTypeKindModifier(this INamedTypeSymbol @this)
		=> (@this.TypeKind, @this.IsRecord) switch
		{
			(Kind.Class, true) => "record",
			(Kind.Class, _) => "class",
			(Kind.Struct, true) => "record struct",
			(Kind.Struct, _) => "struct",
			(Kind.Interface, _) => "interface"
			//(Kind.Delegate, _) => "delegate",
			//(Kind.Enum, _) => "enum"
		};
}
