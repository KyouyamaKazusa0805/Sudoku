using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.RegionLabel;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates a/an (<b>grouped</b>) <b>alternating inference chain</b>
	/// (AIC) technique searcher.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This technique searcher may use the basic searching way to find all AICs and
	/// grouped AICs. For example, this searcher will try to search for all strong
	/// inferences firstly, and then search a weak inference that the candidate is
	/// in the same region or cell with a node in the strong inference in order to
	/// link them.
	/// </para>
	/// <para>
	/// Note that AIC may be static chains, which means that the searcher may just use
	/// static analysis is fine, which is different with dynamic chains.
	/// </para>
	/// </remarks>
	[TechniqueDisplay(nameof(TechniqueCode.Aic))]
	public sealed class AicTechniqueSearcher : ChainTechniqueSearcher
	{
		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 55;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			var list = new ICollection<ChainInference>?[81];
			GetStrongRelations(list, grid);

			// TODO: Get off to on. In other words, traverse the 'list'.
		}


		/// <summary>
		/// Get all strong relations at initial.
		/// </summary>
		/// <param name="list">The collected strong relations grouped by its cell.</param>
		/// <param name="grid">The grid.</param>
		private static void GetStrongRelations(ICollection<ChainInference>?[] list, IReadOnlyGrid grid)
		{
			foreach (int cell in BivalueMap)
			{
				short mask = grid.GetCandidateMask(cell);
				byte d1 = (byte)mask.FindFirstSet();
				byte d2 = (byte)mask.GetNextSet(d1);
				list[cell] = new List<ChainInference>
				{
					new ChainInference(new ChainNode(d1, new GridMap { cell }), new ChainNode(d2, new GridMap { cell }))
				};
			}

			var regions = (Span<int>)stackalloc int[3];
			foreach (int cell in EmptyMap - BivalueMap)
			{
				foreach (byte digit in grid.GetCandidates(cell))
				{
					regions[0] = GetRegion(cell, Row);
					regions[1] = GetRegion(cell, Column);
					regions[2] = GetRegion(cell, Block);
					foreach (int region in regions)
					{
						var map = CandMaps[digit] & RegionMaps[region];
						if (map.Count == 2)
						{
							// Conjugate pair (here will be strong link) found.
							(list[cell] ??= new List<ChainInference>()).Add(
								new ChainInference(
									new ChainNode(digit, new GridMap { map.SetAt(0) }),
									new ChainNode(digit, new GridMap { map.SetAt(1) })));
						}
					}
				}
			}
		}
	}
}
