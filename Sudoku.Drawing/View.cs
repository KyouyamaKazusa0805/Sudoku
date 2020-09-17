using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Sudoku.Data;

namespace Sudoku.Drawing
{
	/// <summary>
	/// Encapsulates a view when displaying the information on forms.
	/// </summary>
	/// <param name="CellOffsets">All cells used.</param>
	/// <param name="CandidateOffsets">All candidates used.</param>
	/// <param name="RegionOffsets">All regions used.</param>
	/// <param name="Links">All links used.</param>
	[DebuggerStepThrough]
	public sealed record View(
		IReadOnlyList<(int Id, int CellOffset)>? CellOffsets,
		IReadOnlyList<(int Id, int CandidateOffset)>? CandidateOffsets,
		IReadOnlyList<(int Id, int RegionOffset)>? RegionOffsets,
		IReadOnlyList<Link>? Links)
	{
		/// <summary>
		/// Provides a new default view list for initialization.
		/// </summary>
		public static readonly View[] DefaultViews = new[] { new View() };


		/// <summary>
		/// Initializes an instance with the specified highlighted candidate offsets.
		/// </summary>
		/// <param name="candidateOffsets">
		/// The list of pairs of identifier and candidate offset.
		/// </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public View(IReadOnlyList<(int, int)> candidateOffsets) : this(null, candidateOffsets, null, null)
		{
		}

		/// <include file='..\GlobalDocComments.xml' path='comments/defaultConstructor'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private View() : this(default, default, default, default)
		{
		}
	}
}
