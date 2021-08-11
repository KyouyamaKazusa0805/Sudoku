using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Solving.Manual.Extensions;
using Sudoku.Solving.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Provides a usage of <b>Bowman's bingo</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="ContradictionSeries">Indicates all conclusions that occurs a contradiction.</param>
	public sealed record BowmanBingoStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		IReadOnlyList<Conclusion> ContradictionSeries
	) : LastResortStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 8.0M + ContradictionSeries.Count.GetExtraDifficultyByLength();

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags =>
			base.TechniqueTags | TechniqueTags.LongChaining | TechniqueTags.ForcingChains;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.BowmanBingo;

		/// <inheritdoc/>
		public override TechniqueGroup TechniqueGroup => TechniqueGroup.BowmanBingo;

		[FormatItem]
		private string ContradictionSeriesStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new ConclusionCollection(ContradictionSeries).ToString(false, " -> ");
		}
	}
}
