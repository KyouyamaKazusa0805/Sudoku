using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;
using static Sudoku.Solving.Constants.Processings;

namespace Sudoku.Solving.Manual.Intersections
{
	/// <summary>
	/// Encapsulates a <b>locked candidates</b> (LC) technique searcher.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.Pointing))]
	[SearcherProperty(26)]
	public sealed class LcTechniqueSearcher : IntersectionTechniqueSearcher
	{
		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			var r = (Span<int>)stackalloc int[2];
			foreach (var ((baseSet, coverSet), (a, b, c)) in IntersectionMaps)
			{
				if (!EmptyMap.Overlaps(c))
				{
					continue;
				}

				short m = (short)(BitwiseOrMasks(grid, c) & (BitwiseOrMasks(grid, a) ^ BitwiseOrMasks(grid, b)));
				if (m == 0)
				{
					continue;
				}

				foreach (int digit in m.GetAllSets())
				{
					GridMap elimMap;
					(r[0], r[1], elimMap) =
						a.Overlaps(CandMaps[digit])
							? (coverSet, baseSet, a & CandMaps[digit])
							: (baseSet, coverSet, b & CandMaps[digit]);
					if (elimMap.IsEmpty)
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					foreach (int cell in elimMap)
					{
						conclusions.Add(new(Elimination, cell, digit));
					}

					var candidateOffsets = new List<(int, int)>();
					foreach (int cell in c & CandMaps[digit])
					{
						candidateOffsets.Add((0, cell * 9 + digit));
					}

					accumulator.Add(
						new LcTechniqueInfo(
							conclusions,
							views: new[] { new View(null, candidateOffsets, new[] { (0, r[0]), (1, r[1]) }, null) },
							digit,
							baseSet: r[0],
							coverSet: r[1]));
				}
			}
		}
	}
}
