using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Manual.LastResorts;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.Fishes
{
	/// <summary>
	/// Encapsulates a <b>Hobiwan's fish</b> technique searcher.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.FrankenSwordfish))]
	public sealed class ComplexFishTechniqueSearcher : FishTechniqueSearcher
	{
		/// <summary>
		/// The maximum size you want to search.
		/// </summary>
		private readonly int _size;


		/// <summary>
		/// Initializes an instance with the specified size.
		/// </summary>
		/// <param name="size">The size.</param>
		public ComplexFishTechniqueSearcher(int size) => _size = size;


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(80);


		/// <inheritdoc/>
		/// <remarks>
		/// <para>We use following algorithm to implement the complex fishes:</para>
		/// <para>
		/// (1) We call the POM searcher (<see cref="PomTechniqueSearcher"/>)
		/// to get all eliminations of a single digit.
		/// If the digit contains the eliminations, we will try to search for complex fishes
		/// (Here complex fish means Hobiwan's fish; In technically, not all POM eliminations
		/// can be found in relative complex fishes).
		/// </para>
		/// <para>
		/// (2) We will try to enumerate all region combinations numbered as the digit <c>0..27</c>.
		/// The size of a combination is the fish size we will find. If <c>size</c> is <c>n</c>, the size
		/// of each region combination is always <c>n</c>; in addition, we will get a new cell map
		/// that contains only cells in base sets, without the elimination cell's peers.
		/// </para>
		/// <para>
		/// (3) Continue to enumerate all region combinations of size <c>n - 1</c> (not <c>n</c>).
		/// If the combination covers all cells in that step (2) got, complex fish must be found.
		/// </para>
		/// </remarks>
		/// <seealso cref="PomTechniqueSearcher"/>
		public override unsafe void GetAll(IList<TechniqueInfo> accumulator, in SudokuGrid grid)
		{
			var globalSteps = new List<TechniqueInfo>();
			new PomTechniqueSearcher().GetAll(globalSteps, grid);
			if (globalSteps.Count == 0)
			{
				return;
			}

			var resultAccumulator = new List<HobiwanFishTechniqueInfo>();
			foreach (int elim in
				from conclusionList in from info in globalSteps select info.Conclusions
				from conclusion in conclusionList
				select conclusion.Cell * 9 + conclusion.Digit)
			{
				GetAll(resultAccumulator, elim);
			}
			if (resultAccumulator.Count == 0)
			{
				return;
			}

			var set = new Set<HobiwanFishTechniqueInfo>(resultAccumulator);
			resultAccumulator.Clear();
			resultAccumulator.AddRange(set);
			resultAccumulator.Sort(&cmp);
			accumulator.AddRange(resultAccumulator);

			static int cmp(in HobiwanFishTechniqueInfo l, in HobiwanFishTechniqueInfo r) =>
				(l.Digit, r.Digit, l.Size, r.Size) switch
				{
					(var ld, var rd, var ls, var rs) => true switch
					{
						_ when ld > rd => 1,
						_ when ld < rd => -1,
						_ when ls > rs => 1,
						_ when ls < rs => -1,
						_ => 0
					}
				};
		}

		/// <summary>
		/// Get all hobiwan fish instance with the specified size you want to search.
		/// </summary>
		/// <param name="accumulator">The result accumulator.</param>
		/// <param name="elim">
		/// The elimination that the possible relative complex fish you want to search.
		/// </param>
		private void GetAll(IList<HobiwanFishTechniqueInfo> accumulator, int elim)
		{
			int elimCell = elim / 9, elimDigit = elim % 9;
			for (int size = 2; size < _size; size++)
			{
				var candMap = CandMaps[elimDigit];
				int baseMask = GetAllRegionsMask(candMap);
				foreach (int[] baseSets in baseMask.GetMaskSubsets(size))
				{
					short mask = 0;
					foreach (int baseSet in baseSets)
					{
						mask |= (short)(1 << baseSet);
					}

					var fishMap = GridMap.Empty;
					foreach (var baseSet in baseSets)
					{
						fishMap |= RegionMaps[baseSet] & candMap;
					}

					var coverMapWithoutPeer = fishMap - new GridMap(elimCell);
					int coverMask = GetAllRegionsMask(coverMapWithoutPeer) & ~mask;
					foreach (int[] coverSets in coverMask.GetMaskSubsets(size - 1))
					{
						var tempMap = GridMap.Empty;
						foreach (var coverSet in coverSets)
						{
							tempMap |= RegionMaps[coverSet] & candMap;
						}
						if (tempMap[elimCell] && (tempMap & coverMapWithoutPeer) != coverMapWithoutPeer)
						{
							continue;
						}

						// Possible fish found. Now check fins.
						GridMap exoFins = fishMap - coverMapWithoutPeer, endoFins = GridMap.Empty;
						foreach (int[] baseSetPair in baseSets.GetSubsets(2))
						{
							endoFins |= candMap & RegionMaps[baseSets[0]] & RegionMaps[baseSets[1]];
						}

						var candidateOffsets = new List<DrawingInfo>();
						candidateOffsets.AddRange(
							from cell in coverMapWithoutPeer select new DrawingInfo(0, cell * 9 + elimDigit));
						candidateOffsets.AddRange(
							from cell in exoFins select new DrawingInfo(1, cell * 9 + elimDigit));
						candidateOffsets.AddRange(
							from cell in endoFins select new DrawingInfo(2, cell * 9 + elimDigit));
						var regionOffsets = new List<DrawingInfo>();
						regionOffsets.AddRange(from baseSet in baseSets select new DrawingInfo(0, baseSet));
						regionOffsets.AddRange(from coverSet in coverSets select new DrawingInfo(1, coverSet));

						accumulator.Add(
							new(
								new Conclusion[] { new(ConclusionType.Elimination, elim) },
								new View[] { new(null, candidateOffsets, regionOffsets, null) },
								elimDigit,
								baseSets,
								coverSets,
								exoFins,
								endoFins,
								IsSashimi: false));
					}
				}
			}
		}

		/// <summary>
		/// Get all regions mask.
		/// </summary>
		/// <param name="map">(<see langword="in"/> parameter) The map.</param>
		/// <returns>The mask.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetAllRegionsMask(in GridMap map) =>
			map.RowMask << 9 | map.ColumnMask << 18 | (int)map.BlockMask;
	}
}
