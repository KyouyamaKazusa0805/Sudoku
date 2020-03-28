using System.Collections.Generic;
using System.Diagnostics;
using Sudoku.Data;

namespace Sudoku.Drawing
{
	/// <summary>
	/// Encapsulates a view when displaying the information on forms.
	/// </summary>
	[DebuggerStepThrough]
	public sealed class View
	{
		/// <include file='../../GlobalDocComments.xml' path='comments/defaultConstructor'/>
		/// <remarks>
		/// The initializer is equivalent to <c>new View(null, null, null, null)</c>
		/// </remarks>
		public View() { }

		/// <summary>
		/// Initializes an instance with information.
		/// </summary>
		/// <param name="cellOffsets">
		/// The list of pairs of identifier and cell offset.
		/// </param>
		/// <param name="candidateOffsets">
		/// The list of pairs of identifier and candidate offset.
		/// </param>
		/// <param name="regionOffsets">
		/// The list of pairs of identifier and region offset.
		/// </param>
		/// <param name="links">The list of links.</param>
		public View(
			IReadOnlyList<(int, int)>? cellOffsets, IReadOnlyList<(int, int)>? candidateOffsets,
			IReadOnlyList<(int, int)>? regionOffsets, IReadOnlyList<Inference>? links) =>
			(CellOffsets, CandidateOffsets, RegionOffsets, Links) = (cellOffsets, candidateOffsets, regionOffsets, links);


		/// <summary>
		/// All cell offsets.
		/// </summary>
		/// <remarks>
		/// This property is a list of pairs of identifier and cell offsets,
		/// where the identifier is an <see cref="int"/> value that can tell
		/// all cell offsets' colors.
		/// </remarks>
		public IReadOnlyList<(int _id, int _cellOffset)>? CellOffsets { get; }

		/// <summary>
		/// All candidate offsets.
		/// </summary>
		/// <remarks>
		/// This property is a list of pairs of identifier and candidate offsets,
		/// where the identifier is an <see cref="int"/> value that can tell
		/// all cell offsets' colors.
		/// </remarks>
		public IReadOnlyList<(int _id, int _candidateOffset)>? CandidateOffsets { get; }

		/// <summary>
		/// All region offsets.
		/// </summary>
		/// <remarks>
		/// This property is a list of pairs of identifier and region offsets,
		/// where the identifier is an <see cref="int"/> value that can tell
		/// all cell offsets' colors.
		/// </remarks>
		public IReadOnlyList<(int _id, int _regionOffset)>? RegionOffsets { get; }

		/// <summary>
		/// All link masks.
		/// </summary>
		public IReadOnlyList<Inference>? Links { get; }
	}
}
