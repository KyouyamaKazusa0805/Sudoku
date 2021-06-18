using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Compilation"/>.
	/// </summary>
	/// <seealso cref="Compilation"/>
	public static class CompilationEx
	{
		/// <summary>
		/// Determine whether the type is an attribute.
		/// </summary>
		/// <param name="this">The compilation.</param>
		/// <param name="symbol">The symbol to check.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public static bool IsAttribute(this Compilation @this, INamedTypeSymbol symbol) =>
			symbol.DerivedFrom(@this.GetTypeByMetadataName("System.Attribute"));
	}
}
