using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;
using static Sudoku.Solving.Utils.ChainingDifficultyRatingUtils;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Provides a usage of bowman bingo technique.
	/// </summary>
	public sealed class BowmanBingoTechniqueInfo : LastResortTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="contradictionSeries">The contradiction series.</param>
		public BowmanBingoTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			IReadOnlyList<Conclusion> contradictionSeries)
			: base(conclusions, views) => ContradictionSeries = contradictionSeries;


		/// <summary>
		/// Indicates all conclusions that occurs a contradict.
		/// </summary>
		public IReadOnlyList<Conclusion> ContradictionSeries { get; }

		/// <inheritdoc/>
		public override string Name => "Bowman Bingo";

		/// <inheritdoc/>
		public override decimal Difficulty =>
			8.0m + GetExtraDifficultyByLength(ContradictionSeries.Count);

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;


		/// <inheritdoc/>
		public override string ToString()
		{
			string contradictionSeriesStr = ConclusionCollection.ToSimpleString(ContradictionSeries, " -> ");
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: Try {contradictionSeriesStr} => {elimStr}";
		}
	}
}
