using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Sudoku.Diagnostics.CodeAnalysis.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="ILocalReferenceOperation"/>.
	/// </summary>
	/// <seealso cref="ILocalReferenceOperation"/>
	public static class ILocalReferenceOperationEx
	{
		/// <summary>
		/// Checks whether the current instance has the same reference with the specified one.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="other">Another instance.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool SameReferenceWith(this ILocalReferenceOperation @this, ILocalReferenceOperation other)
		{
			ILocalSymbol local1 = @this.Local, local2 = other.Local;
			return local1.ToDisplayString() == local2.ToDisplayString();
		}
	}
}
