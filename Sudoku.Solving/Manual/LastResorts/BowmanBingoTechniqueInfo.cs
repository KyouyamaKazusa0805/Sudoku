using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using static Sudoku.Solving.Constants.Processings;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Provides a usage of <b>Bowman's bingo</b> technique.
	/// </summary>
	public sealed class BowmanBingoTechniqueInfo : LastResortTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
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
		public override decimal Difficulty => 8.0M + GetExtraDifficultyByLength(ContradictionSeries.Count);

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.BowmanBingo;


		/// <inheritdoc/>
		public override string ToString()
		{
			string contradictionSeriesStr = new ConclusionCollection(ContradictionSeries).ToString(false, " -> ");
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: Try {contradictionSeriesStr} => {elimStr}";
		}
	}
}
