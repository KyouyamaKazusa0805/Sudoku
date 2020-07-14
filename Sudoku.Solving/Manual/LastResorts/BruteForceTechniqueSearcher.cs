using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Solving.Annotations;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Encapsulates a <b>brute force</b> technique searcher.
	/// The searcher is not executed until all searchers whose priority
	/// is higher than this one cannot find out any technique steps.
	/// </summary>
	/// <remarks>
	/// This searcher is a trick, because it will check the assignments on
	/// the terminal grid (I mean, the answer grid).
	/// </remarks>
	[TechniqueDisplay(nameof(TechniqueCode.BruteForce))]
	[SearcherProperty(200)]
	public sealed class BruteForceTechniqueSearcher : LastResortTechniqueSearcher
	{
		/// <summary>
		/// The order of cell offsets to get values.
		/// </summary>
		private static readonly int[] TryAndErrorOrder =
		{
			64, 63, 62, 61, 60, 59, 58, 57, 56,
			65, 36, 35, 34, 33, 32, 31, 30, 55,
			66, 37, 16, 15, 14, 13, 12, 29, 54,
			67, 38, 17,  4,  3,  2, 11, 28, 53,
			68, 39, 18,  5,  0,  1, 10, 27, 52,
			69, 40, 19,  6,  7,  8,  9, 26, 51,
			70, 41, 20, 21, 22, 23, 24, 25, 50,
			71, 42, 43, 44, 45, 46, 47, 48, 49,
			72, 73, 74, 75, 76, 77, 78, 79, 80
		};


		/// <summary>
		/// The solution.
		/// </summary>
		private readonly IReadOnlyGrid _solution;


		/// <summary>
		/// A trick. Initializes an instance with the solution grid.
		/// This searcher will try to extract a value from the
		/// solution.
		/// </summary>
		/// <param name="solution">The solution.</param>
		public BruteForceTechniqueSearcher(IReadOnlyGrid solution) => _solution = solution;


		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			foreach (int offset in TryAndErrorOrder)
			{
				if (grid.GetStatus(offset) != Empty)
				{
					continue;
				}

				int cand = offset * 9 + _solution[offset];
				accumulator.Add(
					new BruteForceTechniqueInfo(
						conclusions: new[] { new Conclusion(Assignment, cand) },
						views: new[] { new View(new[] { (0, cand) }) }));
			}
		}
	}
}
