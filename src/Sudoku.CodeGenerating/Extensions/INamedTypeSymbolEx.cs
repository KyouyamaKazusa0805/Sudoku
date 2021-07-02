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
	}
}
