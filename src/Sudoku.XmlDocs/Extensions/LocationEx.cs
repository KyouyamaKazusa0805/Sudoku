using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Sudoku.XmlDocs.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Location"/>.
	/// </summary>
	/// <seealso cref="Location"/>
	public static class LocationEx
	{
		/// <summary>
		/// Indicates whether the current location is in the specified range.
		/// </summary>
		/// <param name="this">The location instance.</param>
		/// <param name="min">The minimal value.</param>
		/// <param name="max">The maximal value.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsInRange(this Location @this, int min, int max) =>
			TextSpan.FromBounds(min, max).Contains(@this.SourceSpan);
	}
}
