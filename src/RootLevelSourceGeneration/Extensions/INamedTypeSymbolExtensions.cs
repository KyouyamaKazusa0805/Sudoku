namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides extension methods for <see cref="INamedTypeSymbol"/>.
/// </summary>
/// <seealso cref="INamedTypeSymbol"/>
internal static class INamedTypeSymbolExtensions
{
	/// <summary>
	/// Determines whether the current type is derived from the specified type (a <see langword="class"/>, not <see langword="interface"/>).
	/// </summary>
	/// <param name="this">The current type.</param>
	/// <param name="baseType">The base type to be checked.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool IsDerivedFrom(this INamedTypeSymbol @this, INamedTypeSymbol baseType)
	{
		for (var temp = @this.BaseType; temp is not null; temp = temp.BaseType)
		{
			if (SymbolEqualityComparer.Default.Equals(temp, baseType))
			{
				return true;
			}
		}

		return false;
	}

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
		var result = @this.ToDisplayString(ExtendedSymbolDisplayFormat.FullyQualifiedFormatWithConstraints)[8..];
		scoped var buffer = (stackalloc char[result.Length]);
		buffer.Clear();
		var pointer = 0;
		for (var i = 0; i < result.Length; i++)
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
			(TypeKind.Class, true) => "record",
			(TypeKind.Class, _) => "class",
			(TypeKind.Struct, true) => "record struct",
			(TypeKind.Struct, _) => "struct",
			(TypeKind.Interface, _) => "interface"
			//(TypeKind.Delegate, _) => "delegate",
			//(TypeKind.Enum, _) => "enum"
		};
}
