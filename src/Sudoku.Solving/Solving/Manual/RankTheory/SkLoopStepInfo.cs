using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.RankTheory
{
	/// <summary>
	/// Provides a usage of <b>domino loop</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cells">All cells used.</param>
	public sealed record SkLoopStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, IReadOnlyList<int> Cells
	) : RankTheoryStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 9.6M;

		/// <inheritdoc/>
		public override string? Acronym => "SK-Loop";

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags =>
			base.TechniqueTags | TechniqueTags.LongChaining | TechniqueTags.RankTheory;

		/// <inheritdoc/>
		public override TechniqueGroup TechniqueGroup => TechniqueGroup.SkLoop;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.SkLoop;

		[FormatItem]
		private string CellsCountStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Cells.Count.ToString();
		}

		[FormatItem]
		private string CellsStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Cells(Cells).ToString();
		}
	}
}
