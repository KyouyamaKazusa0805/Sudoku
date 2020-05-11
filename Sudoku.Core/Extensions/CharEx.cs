using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="char"/>.
	/// </summary>
	/// <seealso cref="char"/>
	[DebuggerStepThrough]
	public static class CharEx
	{
		/// <summary>
		/// To determine whether the specified character is a digit character.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The character.</param>
		/// <returns>The <see cref="bool"/> value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsDigit(this char @this) => char.IsDigit(@this);
	}
}
