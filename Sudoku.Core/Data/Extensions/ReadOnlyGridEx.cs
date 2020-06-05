using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Sudoku.Extensions;
using static Sudoku.Data.CellStatus;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="IReadOnlyGrid"/>.
	/// </summary>
	/// <seealso cref="IReadOnlyGrid"/>
	[DebuggerStepThrough]
	public static class ReadOnlyGridEx
	{
		/// <summary>
		/// Convert the current read-only grid to mutable one.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <returns>The mutable one.</returns>
		/// <remarks>
		/// This method is only use type conversion, so the return value has a same
		/// reference with this specified argument holds.
		/// </remarks>
		/// <exception cref="InvalidCastException">
		/// Throws when <see cref="IReadOnlyGrid"/> cannot convert to a <see cref="Grid"/>.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Grid ToMutable(this IReadOnlyGrid @this) =>
			@this is Grid result
				? result
				: throw new InvalidCastException("The specified read-only grid cannot converted to a normal one.");

		/// <summary>
		/// <para>Indicates whether the specified cell is a bivalue cell.</para>
		/// <para>
		/// Note that given and modifiable cells always make this method
		/// return <see langword="false"/>.
		/// </para>
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <param name="cellOffset">The cell offset.</param>
		/// <param name="mask">
		/// (<see langword="out"/> parameter) The result mask. The mask consists of
		/// 9 bits, where the set bits means the digit exists in this cell; otherwise,
		/// the bit will not be set.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsBivalueCell(this IReadOnlyGrid @this, int cellOffset, out short mask)
		{
			mask = 0;
			return @this.GetStatus(cellOffset) == Empty
				&& (mask = @this.GetCandidateMask(cellOffset)).CountSet() == 2;
		}

		/// <summary>
		/// <para>
		/// Indicates whether the specified grid contains the digit in the specified cell.
		/// </para>
		/// <para>
		/// The return value will be <see langword="true"/> if and only if
		/// the cell is empty and contains that digit.
		/// </para>
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <param name="cellOffset">The cell offset.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>
		/// A <see cref="bool"/>? value indicating that.
		/// </returns>
		/// <remarks>
		/// The cases of the return value are below:
		/// <list type="table">
		/// <item>
		/// <term><c><see langword="true"/></c></term>
		/// <description>
		/// The cell is an empty cell <b>and</b> contains the specified digit.
		/// </description>
		/// </item>
		/// <item>
		/// <term><c><see langword="false"/></c></term>
		/// <description>
		/// The cell is an empty cell <b>but doesn't</b> contain the specified digit.
		/// </description>
		/// </item>
		/// <item>
		/// <term><c><see langword="null"/></c></term>
		/// <description>The cell is <b>not</b> an empty cell.</description>
		/// </item>
		/// </list>
		/// </remarks>
		/// <example>
		/// Note that the method will return a <see cref="bool"/>?, so you should use the code
		/// <code>grid.Exists(candidate) is true</code>
		/// or
		/// <code>grid.Exists(candidate) == true</code>
		/// to decide whether a condition is true.
		/// </example>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool? Exists(this IReadOnlyGrid @this, int cellOffset, int digit) =>
			@this.GetStatus(cellOffset) == Empty ? !@this[cellOffset, digit] : (bool?)null;
	}
}
