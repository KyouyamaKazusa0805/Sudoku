using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

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
	[TechniqueDisplay("Brute Force")]
	public sealed class BruteForceTechniqueSearcher : LastResortTechniqueSearcher
	{
		/// <summary>
		/// The order of cell offsets to get values.
		/// </summary>
		private static readonly int[] TryAndErrorOrder = new[]
		{
			40, 41, 50, 49, 48, 39, 30, 31, 32,
			33, 42, 51, 60, 59, 58, 57, 56, 47,
			38, 29, 20, 21, 22, 23, 24, 25, 34,
			43, 52, 61, 70, 69, 68, 67, 66, 65,
			64, 55, 46, 37, 28, 19, 10, 11, 12,
			13, 14, 15, 16, 17, 26, 35, 44, 53,
			62, 71, 80, 79, 78, 77, 76, 75, 74,
			73, 72, 63, 54, 45, 36, 27, 18, 9,
			0, 1, 2, 3, 4, 5, 6, 7, 8
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
		public BruteForceTechniqueSearcher(IReadOnlyGrid solution) =>
			_solution = solution;


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 200;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void AccumulateAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			foreach (int offset in TryAndErrorOrder)
			{
				if (grid.GetCellStatus(offset) != CellStatus.Empty)
				{
					continue;
				}

				int cand = offset * 9 + _solution[offset];
				accumulator.Add(
					new BruteForceTechniqueInfo(
						conclusions: new[] { new Conclusion(ConclusionType.Assignment, cand) },
						views: new[]
						{
							new View(
								cellOffsets: null,
								candidateOffsets: new[] { (0, cand) },
								regionOffsets: null,
								links: null)
						}));
			}
		}
	}
}
