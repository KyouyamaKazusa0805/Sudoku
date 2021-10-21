namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides extension methods for <see cref="INamedTypeSymbol"/>.
/// </summary>
/// <seealso cref="INamedTypeSymbol"/>
internal static class INamedTypeSymbolExtensions
{
	public static SymbolOutputInfo GetSymbolOutputInfo(this INamedTypeSymbol @this, bool checkNotRefStruct = false)
	{
		string typeName = @this.Name;
		string fullTypeName = @this.ToDisplayString(TypeFormats.FullNameWithConstraints);
		string namespaceName = @this.ContainingNamespace.ToDisplayString();

		int i = fullTypeName.IndexOf('<');
		bool isGeneric = i != -1;
		string genericParametersList = i == -1 ? string.Empty : fullTypeName.Substring(i);

		int j = fullTypeName.IndexOf('>');
		string genericParametersListWithoutConstraint = i == -1 ? string.Empty : fullTypeName.Substring(i, j - i + 1);

		string typeKind = (@this.IsRecord, @this.TypeKind) switch
		{
			(IsRecord: true, TypeKind: TypeKind.Class) => "record ",
			(IsRecord: true, TypeKind: TypeKind.Struct) => "record struct ",
			(IsRecord: false, TypeKind: TypeKind.Class) => "class ",
			(IsRecord: false, TypeKind: TypeKind.Struct) => "struct "
		};
		string readonlyKeyword = (
			checkNotRefStruct
			? @this is { TypeKind: TypeKind.Struct, IsRefLikeType: false, IsReadOnly: false }
			: @this is { TypeKind: TypeKind.Struct, IsReadOnly: false }
		) ? "readonly " : string.Empty;
		string inKeyword = @this.TypeKind == TypeKind.Struct ? "in " : string.Empty;
		string nullableAnnotation = @this.TypeKind == TypeKind.Class ? "?" : string.Empty;

		return new(
			typeName, fullTypeName, namespaceName, genericParametersList,
			genericParametersListWithoutConstraint, typeKind, readonlyKeyword,
			inKeyword, nullableAnnotation, isGeneric
		);
	}

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
	/// <param name="symbol">The symbol to check.</param>
	/// <param name="compilation">The compilation.</param>
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
	public static bool CheckAnyTypeArgumentIsMarked<TAttribute>(
		this INamedTypeSymbol? symbol,
		Compilation compilation
	) where TAttribute : Attribute
	{
		var obsolete = compilation.GetTypeByMetadataName(typeof(TAttribute).FullName)!;
		Func<ISymbol?, ISymbol?, bool> e = SymbolEqualityComparer.Default.Equals;

		return f(symbol);


		bool f(INamedTypeSymbol? symbol) => symbol switch
		{
			null => false,
			{ IsGenericType: false } => symbol.GetAttributes().Any(a => e(a.AttributeClass, obsolete)),
			{ TypeArguments: var typeArgs } => typeArgs.All(t => f(t as INamedTypeSymbol))
		};
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
		string result = @this.ToDisplayString(TypeFormats.FullNameWithConstraints);
		var buffer = (stackalloc char[result.Length]);
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

		return buffer.Slice(0, pointer).ToString();
	}
}
