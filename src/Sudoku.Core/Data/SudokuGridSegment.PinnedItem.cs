#if SOLUTION_WIDE_CODE_ANALYSIS
using System.Diagnostics.CodeAnalysis;
#endif

namespace Sudoku.Data
{
	partial struct SudokuGridSegment
	{
		/// <summary>
		/// Indicates the item that <see cref="GetPinnableReference()"/> selects and returns the reference.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This enumeration type is called to select what the inner field we want to fix. For example,
		/// if called <see cref="GetPinnableReference()"/> (parameterless method), the default value is
		/// <see cref="Masks"/>, and we'll get the table that contains all current statuses of the
		/// curect collection using a <see cref="short"/>*. The code should be:
		/// <code>
		/// fixed (short* s = segment)
		/// {
		///     // Do something...
		/// }
		/// </code>
		/// The code is equivalent to
		/// <code><![CDATA[
		/// fixed (short* s = &segment.GetPinnableReference())
		/// {
		///     // Do something...
		/// }
		/// ]]></code>
		/// </para>
		/// <para>
		/// If you want to get the item <see cref="CandidateMasks"/>, you should call the method
		/// <see cref="GetPinnableReference(PinnedItem)"/>.
		/// </para>
		/// </remarks>
		/// <see cref="GetPinnableReference()"/>
		/// <see cref="GetPinnableReference(PinnedItem)"/>
#if SOLUTION_WIDE_CODE_ANALYSIS
		[Closed]
#endif
		public enum PinnedItem
		{
			/// <summary>
			/// Indicates the pinned item is <c>_maskList</c>.
			/// </summary>
			Masks,

			/// <summary>
			/// Indicates the pinned item is <c>_candidateList</c>.
			/// </summary>
			CandidateMasks
		}
	}
}
