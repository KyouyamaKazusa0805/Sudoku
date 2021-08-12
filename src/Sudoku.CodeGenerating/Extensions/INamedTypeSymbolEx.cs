namespace Sudoku.CodeGenerating.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="INamedTypeSymbol"/>.
	/// </summary>
	/// <seealso cref="INamedTypeSymbol"/>
	public static class INamedTypeSymbolEx
	{
		/// <summary>
		/// Get the type kind string.
		/// </summary>
		/// <param name="this">The symbol.</param>
		/// <returns>The type kind string.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetTypeKindString(this INamedTypeSymbol @this) =>
			(@this.IsRecord, @this.TypeKind) switch
			{
				(IsRecord: true, TypeKind: TypeKind.Class) => "record ",
				(IsRecord: true, TypeKind: TypeKind.Struct) => "record struct ",
				(IsRecord: false, TypeKind: TypeKind.Class) => "class ",
				(IsRecord: false, TypeKind: TypeKind.Struct) => "struct "
			};

		/// <summary>
		/// Indicates whether the member should append <see langword="readonly"/> modifier.
		/// </summary>
		/// <param name="this">The type symbol.</param>
		/// <param name="checkNotRefStruct">
		/// Indicates whether the method should check whether the type is a <see langword="ref struct"/>
		/// and not a <see langword="ref struct"/>.
		/// </param>
		/// <returns>A <see cref="bool"/> result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool MemberShouldAppendReadOnly(this INamedTypeSymbol @this, bool checkNotRefStruct = false) =>
			checkNotRefStruct
			? @this is { TypeKind: TypeKind.Struct, IsRefLikeType: false, IsReadOnly: false }
			: @this is { TypeKind: TypeKind.Struct, IsReadOnly: false };

		/// <summary>
		/// Indicates whether the member should append <see langword="in"/> modifier.
		/// </summary>
		/// <param name="this">The type symbol.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool MemberShouldAppendIn(this INamedTypeSymbol @this) =>
			@this is { TypeKind: TypeKind.Struct };

		/// <summary>
		/// Get the detail information that represented as <see cref="string"/> values.
		/// </summary>
		/// <param name="this">The symbol.</param>
		/// <param name="checkNotRefStruct">
		/// Indicates whether the method will check whether the type is a <see langword="ref struct"/>.
		/// </param>
		/// <param name="fullTypeName">The full type name.</param>
		/// <param name="namespaceName">The namespace name.</param>
		/// <param name="genericParametersList">
		/// The generic parameter list. The type parameter constraint will also include,
		/// e.g. <c><![CDATA[Namespace.TypeName<T> where T : class?]]></c>.
		/// </param>
		/// <param name="genericParametersListWithoutConstraint">
		/// The generic parameter list without type parameter constraint,
		/// e.g. <c><![CDATA[Namespace.TypeName<T>]]></c>.
		/// </param>
		/// <param name="typeKind">The type kind, e.g. <c>struct</c>.</param>
		/// <param name="readonlyKeyword">The read-only keyword on members.</param>
		/// <param name="isGeneric">Indicates whether the type is a generic type.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void DeconstructInfo(
			this INamedTypeSymbol @this, bool checkNotRefStruct, out string fullTypeName, out string namespaceName,
			out string genericParametersList, out string genericParametersListWithoutConstraint,
			out string typeKind, out string readonlyKeyword, out bool isGeneric)
		{
			fullTypeName = @this.ToDisplayString(FormatOptions.TypeFormat);
			namespaceName = @this.ContainingNamespace.ToDisplayString();

			int i = fullTypeName.IndexOf('<');
			isGeneric = i != -1;
			genericParametersList = i == -1 ? string.Empty : fullTypeName.Substring(i);

			int j = fullTypeName.IndexOf('>');
			genericParametersListWithoutConstraint = i == -1 ? string.Empty : fullTypeName.Substring(i, j - i + 1);

			typeKind = @this.GetTypeKindString();
			readonlyKeyword = @this.MemberShouldAppendReadOnly(checkNotRefStruct) ? "readonly " : string.Empty;
		}

		/// <summary>
		/// Get all base types of this instance.
		/// </summary>
		/// <param name="this">The type.</param>
		/// <returns>All base types.</returns>
		public static IEnumerable<ISymbol> GetBaseTypes(this INamedTypeSymbol @this)
		{
			for (var s = @this; s is not null; s = s.BaseType)
			{
				yield return s;
			}
		}

		/// <summary>
		/// Get the attribute string representation from the specified type symbol.
		/// </summary>
		/// <param name="this">The type symbol.</param>
		/// <param name="attributeSymbol">The attribute symbol to check.</param>
		/// <returns>The result string.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string? GetAttributeString(this INamedTypeSymbol @this, ISymbol? attributeSymbol) => (
			from attribute in @this.GetAttributes()
			where SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeSymbol)
			select attribute
		).FirstOrDefault()?.ToString();

		/// <summary>
		/// Get the attribute strings from the specified type symbol.
		/// </summary>
		/// <param name="this">The type symbol.</param>
		/// <param name="attributeSymbol">The attribute symbol to check.</param>
		/// <returns>The result string.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static IEnumerable<string?> GetAttributeStrings(
			this INamedTypeSymbol @this, ISymbol? attributeSymbol) =>
			from attribute in @this.GetAttributes()
			where SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeSymbol)
			select attribute.ToString();

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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static string ToFileName(this INamedTypeSymbol @this)
		{
			string result = @this.ToDisplayString(FormatOptions.TypeFormat);
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
}
