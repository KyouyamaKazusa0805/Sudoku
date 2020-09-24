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
	/// <param name="DirectLines">All direct lines.</param>
	[DebuggerStepThrough]
	public sealed record View(
		IReadOnlyList<DrawingInfo>? Cells, IReadOnlyList<DrawingInfo>? Candidates,
		IReadOnlyList<DrawingInfo>? Regions, IReadOnlyList<Link>? Links,
		IReadOnlyList<(GridMap Start, GridMap End)>? DirectLines)
	{
		/// <summary>
		/// Provides a new default view list for initialization.
		/// </summary>
		public static readonly View[] DefaultViews = new View[] { new() };


		/// <summary>
		/// Initializes an instance with the specified highlighted candidate offsets.
		/// </summary>
		/// <param name="candidates">
		/// The list of pairs of identifier and candidate offset.
		/// </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public View(IReadOnlyList<DrawingInfo> candidates) : this(null, candidates, null, null, null)
		{
		}

		/// <summary>
		/// Initializes an instance with the specified cells, candidates, regions and links.
		/// </summary>
		/// <param name="cells">The cells.</param>
		/// <param name="candidates">The candidates.</param>
		/// <param name="regions">The regions.</param>
		/// <param name="links">The links.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public View(
			IReadOnlyList<DrawingInfo>? cells, IReadOnlyList<DrawingInfo>? candidates,
			IReadOnlyList<DrawingInfo>? regions, IReadOnlyList<Link>? links) : this(cells, candidates, regions, links, null)
		{
		}

		/// <inheritdoc cref="DefaultConstructor"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private View() : this(null, null, null, null, null)
		{
		}
	}
}
