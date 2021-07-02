using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;

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
		public static string GetTypeKindString(this INamedTypeSymbol @this) => @this switch
		{
			{ IsRecord: true } => "record ",
			{ TypeKind: TypeKind.Class } => "class ",
			{ TypeKind: TypeKind.Struct } => "struct "
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
		/// e.g. <c>Namespace.TypeName&lt;T&gt; where T : class?</c>.
		/// </param>
		/// <param name="genericParametersListWithoutConstraint">
		/// The generic parameter list without type parameter constraint,
		/// e.g. <c>Namespace.TypeName&lt;T&gt;</c>.
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
	}
}
