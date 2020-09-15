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
			40, 41, 32, 31, 30, 39, 48, 49, 50,
			51, 42, 33, 24, 23, 22, 21, 20, 29,
			38, 47, 56, 57, 58, 59, 60, 61, 52,
			43, 34, 25, 16, 15, 14, 13, 12, 11,
			10, 19, 28, 37, 46, 55, 64, 65, 66,
			67, 68, 69, 70, 71, 62, 53, 44, 35,
			26, 17,  8,  7,  6,  5,  4,  3,  2,
			 1,  0,  9, 18, 27, 36, 45, 54, 63,
			72, 73, 74, 75, 76, 77, 78, 79, 80
		};


		/// <summary>
		/// The solution.
		/// </summary>
		private readonly Grid _solution;


		/// <summary>
		/// A trick. Initializes an instance with the solution grid.
		/// This searcher will try to extract a value from the
		/// solution.
		/// </summary>
		/// <param name="solution">The solution.</param>
		public BruteForceTechniqueSearcher(Grid solution) => _solution = solution;


		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, Grid grid)
		{
			foreach (int offset in TryAndErrorOrder)
			{
				if (grid.GetStatus(offset) == Empty)
				{
					int cand = offset * 9 + _solution[offset];
					accumulator.Add(
						new BruteForceTechniqueInfo(
							new Conclusion[] { new(Assignment, cand) },
							new View[] { new(new[] { (0, cand) }) }));
				}
			}
		}
	}
}
