using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.DocComments;

namespace Sudoku.Drawing
{
	/// <summary>
	/// Encapsulates a view when displaying the information on forms.
	/// </summary>
	/// <param name="Cells">All cells used.</param>
	/// <param name="Candidates">All candidates used.</param>
	/// <param name="Regions">All regions used.</param>
	/// <param name="Links">All links used.</param>
	[DebuggerStepThrough]
	public sealed record View(
		IReadOnlyList<DrawingInfo>? Cells, IReadOnlyList<DrawingInfo>? Candidates,
		IReadOnlyList<DrawingInfo>? Regions, IReadOnlyList<Link>? Links)
	{
		/// <summary>
		/// Provides a new default view list for initialization.
		/// </summary>
		public static readonly View[] DefaultViews = new View[] { new() };


		/// <summary>
		/// Initializes an instance with the specified highlighted candidate offsets.
		/// </summary>
		/// <param name="candidateOffsets">
		/// The list of pairs of identifier and candidate offset.
		/// </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public View(IReadOnlyList<DrawingInfo> candidateOffsets) : this(null, candidateOffsets, null, null)
		{
		}

		/// <inheritdoc cref="DefaultConstructor"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private View() : this(default, default, default, default)
		{
		}
	}
}
