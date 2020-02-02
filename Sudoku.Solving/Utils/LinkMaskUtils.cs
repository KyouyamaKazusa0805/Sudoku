using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Provides extension method used for link masks.
	/// </summary>
	[DebuggerStepThrough]
	public static class LinkMaskUtils
	{
		/// <summary>
		/// Create a link mask value with the specified cell offsets
		/// and a type.
		/// </summary>
		/// <param name="c1">The cell offset 1.</param>
		/// <param name="digit1">The candidate digit 1.</param>
		/// <param name="c2">The cell offset 2.</param>
		/// <param name="digit2">The candidate digit 2.</param>
		/// <param name="type">
		/// The type of this link.
		/// This value should be in range 0 to 3.
		/// </param>
		/// <returns>The link mask.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Create(int c1, int digit1, int c2, int digit2, int type) =>
			type << 20 + c1 * 9 + digit1 << 10 + c2 * 9 + digit2;
	}
}
