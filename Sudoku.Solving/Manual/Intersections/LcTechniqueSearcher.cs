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
	[TechniqueDisplay("Locked Candidates")]
	public sealed class LcTechniqueSearcher : IntersectionTechniqueSearcher
	{
		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 26;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			var r = (Span<int>)stackalloc int[2];
			foreach (var ((baseSet, coverSet), (a, b, c)) in IntersectionMaps)
			{
				if (!EmptyMap.Overlaps(c))
				{
					continue;
				}

				short m1 = BitwiseOrMasks(grid, a);
				short m2 = BitwiseOrMasks(grid, b);
				short m3 = BitwiseOrMasks(grid, c);
				short m = (short)(m3 & (m1 ^ m2));
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
					foreach (int cell in elimMap.Offsets)
					{
						conclusions.Add(new Conclusion(Elimination, cell, digit));
					}

					var candidateOffsets = new List<(int, int)>();
					foreach (int cell in (c & CandMaps[digit]).Offsets)
					{
						candidateOffsets.Add((0, cell * 9 + digit));
					}

					accumulator.Add(
						new LcTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets,
									regionOffsets: new[] { (0, r[0]), (1, r[1]) },
									links: null)
							},
							digit,
							baseSet: r[0],
							coverSet: r[1]));
				}
			}
		}
	}
}
