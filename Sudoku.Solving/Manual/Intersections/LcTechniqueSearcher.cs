using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Extensions;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Intersections
{
	/// <summary>
	/// Encapsulates a <b>locked candidates</b> (LC) technique searcher.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.Pointing))]
	public sealed class LcTechniqueSearcher : IntersectionTechniqueSearcher
	{
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(26);


		/// <inheritdoc/>
		[SkipLocalsInit]
		public override void GetAll(IList<TechniqueInfo> accumulator, in SudokuGrid grid)
		{
			var r = (stackalloc int[2]);
			foreach (var ((baseSet, coverSet), (a, b, c)) in IntersectionMaps)
			{
				if (!EmptyMap.Overlaps(c))
				{
					continue;
				}

				short m = (short)(grid.BitwiseOrMasks(c) & (grid.BitwiseOrMasks(a) ^ grid.BitwiseOrMasks(b)));
				if (m == 0)
				{
					continue;
				}

				foreach (int digit in m)
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

					var candidateOffsets = new List<DrawingInfo>();
					foreach (int cell in c & CandMaps[digit])
					{
						candidateOffsets.Add(new(0, cell * 9 + digit));
					}

					accumulator.Add(
						new LcTechniqueInfo(
							conclusions,
							new View[]
							{
								new(
									null,
									candidateOffsets,
									new DrawingInfo[] { new(0, r[0]), new(1, r[1]) },
									null)
							},
							digit,
							r[0],
							r[1]));
				}
			}
		}
	}
}
