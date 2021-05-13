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
		/// Creates a pointer type symbol by the specified special type.
		/// </summary>
		/// <param name="this">The compilation.</param>
		/// <param name="specialType">The special type.</param>
		/// <returns>The result symbol.</returns>
		public static IPointerTypeSymbol GetPointerTypeSymbol(this Compilation @this, SpecialType specialType) =>
			@this.CreatePointerTypeSymbol(@this.GetSpecialType(specialType));
	}
}
