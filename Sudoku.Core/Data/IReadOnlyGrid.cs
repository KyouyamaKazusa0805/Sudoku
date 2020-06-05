using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data.Extensions;
using Sudoku.Extensions;

namespace Sudoku.Data
{
	/// <summary>
	/// Provides a read-only sudoku grid.
	/// </summary>
	public interface IReadOnlyGrid : IEnumerable<short>, IFormattable
	{
		/// <summary>
		/// Indicates the grid has already solved. If the value is <see langword="true"/>,
		/// the grid is solved; otherwise, <see langword="false"/>.
		/// </summary>
		bool HasSolved { get; }

		/// <summary>
		/// Indicates the number of total candidates.
		/// </summary>
		int CandidatesCount { get; }

		/// <summary>
		/// Indicates the current number of the empty cells.
		/// </summary>
		int EmptyCellsCount { get; }


		/// <summary>
		/// Gets or sets a digit into a cell.
		/// </summary>
		/// <param name="cell">The cell offset you want to get or set.</param>
		/// <value>
		/// The digit you want to set. This value should be between 0 and 8.
		/// In addition, if your input is -1, the candidate mask in this cell
		/// will be re-computed. If your input is none of them above, this indexer
		/// will do nothing.
		/// </value>
		/// <returns>
		/// An <see cref="int"/> value indicating the result.
		/// If the current cell does not have a digit
		/// (i.e. The cell is <see cref="CellStatus.Empty"/>),
		/// The value will be -1.
		/// </returns>
		int this[int cell] { get; }

		/// <summary>
		/// Gets or sets a candidate existence case with a <see cref="bool"/> value.
		/// </summary>
		/// <param name="cell">The cell offset between 0 and 80.</param>
		/// <param name="digit">The digit between 0 and 8.</param>
		/// <value>
		/// The case you want to set. <see langword="true"/> means that this candidate
		/// does not exist in this current sudoku grid; otherwise, <see langword="false"/>.
		/// </value>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		bool this[int cell, int digit] { get; }


		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="object"]'/>
		bool Equals(object? obj);

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="GetHashCode"]'/>
		int GetHashCode();

		/// <summary>
		/// Serializes this instance to an array, where all digit value will be stored.
		/// </summary>
		/// <returns>
		/// This array. All elements are between 0 to 9, where 0 means the
		/// cell is <see cref="CellStatus.Empty"/> now.
		/// </returns>
		int[] ToArray();

		/// <summary>
		/// Get a mask of the specified cell.
		/// </summary>
		/// <param name="offset">The cell offset you want to get.</param>
		/// <returns>The mask.</returns>
		short GetMask(int offset);

		/// <summary>
		/// Get the candidate mask part of the specified cell.
		/// </summary>
		/// <param name="cell">The cell offset you want to get.</param>
		/// <returns>
		/// The candidate mask. The return value is a 9-bit <see cref="short"/>
		/// value, where the bit will be <c>0</c> if the corresponding digit <b>does not exist</b> in the cell,
		/// and will be <c>1</c> if the corresponding contains this digit (either the cell
		/// is filled with this digit or the cell is an empty cell, whose candidates contains the digit).
		/// </returns>
		/// <remarks>
		/// Please note that the grid masks is represented with bits, where 0 is for the digit containing in a
		/// cell, 1 is for the digit <b>not</b> containing. However, here the return mask is the reversal:
		/// 1 is for containing and 0 is for <b>not</b>.
		/// </remarks>
		short GetCandidateMask(int cell);

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="string"]'/>
		string ToString(string format);

		/// <summary>
		/// Get the current status for the specified cell.
		/// </summary>
		/// <param name="cell">The cell offset you want to get.</param>
		/// <returns>The cell status.</returns>
		CellStatus GetStatus(int cell);

		/// <summary>
		/// Get all candidates containing in the specified cell.
		/// </summary>
		/// <param name="cell">The cell you want to get.</param>
		/// <returns>All candidates.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerable<int> GetCandidates(int cell) => GetCandidateMask(cell).GetAllSets();

		/// <summary>
		/// Formats the value of the current instance using the specified format.
		/// </summary>
		/// <returns>The string.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString() => ToString(null, null);

		/// <summary>
		/// Creates a new instance that is a copy of the current instance.
		/// </summary>
		/// <returns>The cloneation.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Grid Clone() => this.ToMutable().Clone();

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		string IFormattable.ToString(string? format, IFormatProvider? formatProvider) =>
			this.ToMutable().ToString(format, formatProvider);
	}
}
