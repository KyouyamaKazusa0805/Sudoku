#pragma warning disable IDE0060

using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Solving.Extensions;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Provides a usage of <b>Bowman's bingo</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="ContradictionSeries">Indicates all conclusions that occurs a contradict.</param>
	public sealed record BowmanBingoTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, IReadOnlyList<Conclusion> ContradictionSeries)
		: LastResortTechniqueInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 8.0M + ContradictionSeries.Count.GetExtraDifficultyByLength();

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.BowmanBingo;


		/// <inheritdoc/>
		public override string ToString()
		{
			using var contraditions = new ConclusionCollection(ContradictionSeries);
			string contradictionSeriesStr = contraditions.ToString(false, " -> ");
			using var elims = new ConclusionCollection(Conclusions);
			string elimStr = elims.ToString();
			return $"{Name}: Try {contradictionSeriesStr} => {elimStr}";
		}
	}
}
