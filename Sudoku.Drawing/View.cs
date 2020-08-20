#if CSHARP_9_PREVIEW
#pragma warning disable CS1591
#endif

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Sudoku.Data;

namespace Sudoku.Drawing
{
	/// <summary>
	/// Encapsulates a view when displaying the information on forms.
	/// </summary>
	[DebuggerStepThrough]
	public sealed record View(
		IReadOnlyList<(int _id, int _cellOffset)>? CellOffsets,
		IReadOnlyList<(int _id, int _candidateOffset)>? CandidateOffsets,
		IReadOnlyList<(int _id, int _regionOffset)>? RegionOffsets,
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

#if false
	/// <summary>
	/// Encapsulates a view when displaying the information on forms.
	/// </summary>
	[DebuggerStepThrough]
	public sealed class View
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
			IReadOnlyList<(int, int)>? regionOffsets, IReadOnlyList<Link>? links) =>
			(CellOffsets, CandidateOffsets, RegionOffsets, Links) = (cellOffsets, candidateOffsets, regionOffsets, links);

		/// <include file='..\GlobalDocComments.xml' path='comments/defaultConstructor'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private View()
		{
		}


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
		public IReadOnlyList<Link>? Links { get; }
	}
#endif
}
