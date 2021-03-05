using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Models;
using Sudoku.Techniques;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Intersections
{
	/// <summary>
	/// Encapsulates a <b>locked candidates</b> (LC) technique searcher.
	/// </summary>
	public sealed class LcStepSearcher : IntersectionStepSearcher
	{
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(2, nameof(Technique.Pointing))
		{
			DisplayLevel = 1
		};


		/// <inheritdoc/>
		public override unsafe void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			int* r = stackalloc int[2];
			foreach (var ((baseSet, coverSet), (a, b, c, _)) in IntersectionMaps)
			{
				if ((EmptyMap & c).IsEmpty)
				{
					continue;
				}

				short maskA = 0, maskB = 0, maskC = 0;
				foreach (int cell in a) maskA |= grid.GetCandidates(cell);
				foreach (int cell in b) maskB |= grid.GetCandidates(cell);
				foreach (int cell in c) maskC |= grid.GetCandidates(cell);

				short m = (short)(maskC & (maskA ^ maskB));
				if (m == 0)
				{
					continue;
				}

				foreach (int digit in m)
				{
					Cells elimMap;
					if (!(a & CandMaps[digit]).IsEmpty)
					{
						r[0] = coverSet;
						r[1] = baseSet;
						elimMap = a & CandMaps[digit];
					}
					else
					{
						r[0] = baseSet;
						r[1] = coverSet;
						elimMap = b & CandMaps[digit];
					}
					if (elimMap.IsEmpty)
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					foreach (int cell in elimMap)
					{
						conclusions.Add(new(ConclusionType.Elimination, cell, digit));
					}

					var candidateOffsets = new List<DrawingInfo>();
					foreach (int cell in c & CandMaps[digit])
					{
						candidateOffsets.Add(new(0, cell * 9 + digit));
					}

					accumulator.Add(
						new LcStepInfo(
							conclusions,
							new View[]
							{
								new()
								{
									Candidates = candidateOffsets,
									Regions = new DrawingInfo[] { new(0, r[0]), new(2, r[1]) }
								}
							},
							digit,
							r[0],
							r[1]));
				}
			}
		}
	}
}
