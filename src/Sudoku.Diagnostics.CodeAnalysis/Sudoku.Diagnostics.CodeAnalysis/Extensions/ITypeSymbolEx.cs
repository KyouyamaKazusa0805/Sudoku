using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGen.Deconstruction.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="ITypeSymbol"/>.
	/// </summary>
	/// <seealso cref="ITypeSymbol"/>
	public static class ITypeSymbolEx
	{
		/// <summary>
		/// Get all deconstruction methods in this current type.
		/// </summary>
		/// <param name="this">The type.</param>
		/// <returns>All possible deconstruction methods.</returns>
		public static IEnumerable<IMethodSymbol> GetAllDeconstructionMethods(this ITypeSymbol @this) =>
			from method in @this.GetAllMembers().OfType<IMethodSymbol>()
			where method.IsDeconstructionMethod()
			orderby method.Parameters.Length
			select method;

		/// <summary>
		/// To determine whether the specified symbol is a nullable value type (i.e. <see cref="Nullable{T}"/>).
		/// </summary>
		/// <param name="this">The symbol.</param>
		/// <param name="compilation">
		/// The compilation that is used for constructing a nullable value type.
		/// </param>
		/// <returns>A <see cref="bool"/> result.</returns>
		/// <seealso cref="Nullable{T}"/>
		public static bool IsNullableValueType(this ITypeSymbol @this, Compilation compilation) =>
			SymbolEqualityComparer.Default.Equals(
				compilation.GetTypeByMetadataName("System.Nullable`1"),
				@this.OriginalDefinition
			);

		/// <summary>
		/// To determine whether the specified symbol is a nullable reference type.
		/// </summary>
		/// <param name="this">The symbol.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool IsNullableReferenceType(this ITypeSymbol @this) =>
			@this.NullableAnnotation == NullableAnnotation.Annotated;

		/// <summary>
		/// Determine whether the current type symbol is a nullable type. The nullable types
		/// are:
		/// <list type="number">
		/// <item>Nullable value type (abbr. NVT), i.e. a <see cref="Nullable{T}"/> instance.</item>
		/// <item>Nullable reference type (abbr. NRT).</item>
		/// </list>
		/// </summary>
		/// <param name="this">The symbol to check.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		/// <seealso cref="Nullable{T}"/>
		public static bool IsNullableType(this ITypeSymbol @this) =>
			@this.ToString() is var str && str[str.Length - 1] == '?';
	}
}
