using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Singles
{
	/// <summary>
	/// Encapsulates a <b>single</b> technique searcher.
	/// </summary>
	[TechniqueDisplay("Singles")]
	public sealed class SingleTechniqueSearcher : TechniqueSearcher
	{
		/// <summary>
		/// Indicates the solver enables these options.
		/// </summary>
		private readonly bool _enableFullHouse, _enableLastDigit;


		/// <summary>
		/// Initializes an instance with enable options.
		/// </summary>
		/// <param name="enableFullHouse">
		/// Indicates whether the solver enables full house.
		/// </param>
		/// <param name="enableLastDigit">
		/// Indicates whether the solver enables last digit.
		/// </param>
		public SingleTechniqueSearcher(bool enableFullHouse, bool enableLastDigit) =>
			(_enableFullHouse, _enableLastDigit) = (enableFullHouse, enableLastDigit);


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 10;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			// Search for full houses.
			if (_enableFullHouse)
			{
				for (int region = 0; region < 27; region++)
				{
					var map = RegionMaps[region] & EmptyMap;
					if (map.Count == 1)
					{
						int cell = map.SetAt(0);
						int digit = grid.GetCandidatesReversal(cell).FindFirstSet();
						accumulator.Add(
							new FullHouseTechniqueInfo(
								conclusions: new[] { new Conclusion(Assignment, cell, digit) },
								views: new[]
								{
									new View(
										cellOffsets: null,
										candidateOffsets: new[] { (0, cell * 9 + digit) },
										regionOffsets: new[] { (0, region) },
										links: null)
								},
								cellOffset: cell,
								digit));
					}
				}
			}

			// Search for hidden singles & last digits.
			for (int digit = 0; digit < 9; digit++)
			{
				for (int region = 0; region < 27; region++)
				{
					var map = RegionMaps[region] & CandMaps[digit];
					if (map.Count == 1)
					{
						bool enableAndIsLastDigit = false;
						var cellOffsets = new List<(int, int)>();
						if (_enableLastDigit)
						{
							// Sum up the number of appearing in the grid of 'digit'.
							int digitCount = 0;
							for (int i = 0; i < 81; i++)
							{
								if (grid[i] == digit)
								{
									digitCount++;
									cellOffsets.Add((0, i));
								}
							}

							enableAndIsLastDigit = digitCount == 8;
						}

						int cell = map.SetAt(0);
						accumulator.Add(
							new HiddenSingleTechniqueInfo(
								conclusions: new[] { new Conclusion(Assignment, cell, digit) },
								views: new[]
								{
									new View(
										cellOffsets: enableAndIsLastDigit ? cellOffsets : null,
										candidateOffsets: new[] { (0, cell * 9 + digit) },
										regionOffsets: enableAndIsLastDigit ? null : new[] { (0, region) },
										links: null)
								},
								regionOffset: region,
								cellOffset: cell,
								digit,
								enableAndIsLastDigit));
					}
				}
			}

			// Search for naked singles.
			foreach (int cell in EmptyMap.Offsets)
			{
				short mask = grid.GetCandidatesReversal(cell);
				if (grid.GetStatus(cell) != CellStatus.Empty || !mask.IsPowerOfTwo())
				{
					// 'a.IsPowerOfTwo()' is equivalent to the formula: a & (a - 1) == 0,
					// which means the number 'a' has only one bit is set.
					// For example, a value is 0b001_000_000 in binary, then:
					//     a: 0b001_000_000
					// a - 1: 0b000_111_111
					//     &: 0b000_000_000
					// If and only if the result is 0, the value will contain
					// only one set bit.
					continue;
				}

				int digit = mask.FindFirstSet();
				accumulator.Add(
					new NakedSingleTechniqueInfo(
						conclusions: new[] { new Conclusion(Assignment, cell, digit) },
						views: new[]
						{
							new View(
								cellOffsets: null,
								candidateOffsets: new[] { (0, cell * 9 + digit) },
								regionOffsets: null,
								links: null)
						},
						cellOffset: cell,
						digit));
			}
		}
	}
}
