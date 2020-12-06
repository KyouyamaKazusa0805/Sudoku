using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Extended
{
	/// <summary>
	/// Provides a usage of <b>extended rectangle</b> (XR) type 3 technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="DigitsMask">All digits mask.</param>
	/// <param name="ExtraCells">All extra cells.</param>
	/// <param name="ExtraDigitsMask">All extra digits mask.</param>
	/// <param name="Region">The region.</param>
	public sealed record XrType3TechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in GridMap Cells, short DigitsMask,
		IReadOnlyList<int> ExtraCells, short ExtraDigitsMask, int Region)
		: XrTechniqueInfo(Conclusions, Views, Cells, DigitsMask)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 4.5M + DifficultyExtra[Size] + .1M * ExtraDigitsMask.PopCount();

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.XrType3;


		/// <inheritdoc/>
		public override string ToString() => base.ToString();

		/// <inheritdoc/>
		protected override string GetAdditional()
		{
			string digitsStr = new DigitCollection(ExtraDigitsMask.GetAllSets()).ToString();
			string cellsStr = new GridMap(ExtraCells).ToString();
			string regionStr = new RegionCollection(Region).ToString();
			return $"{digitsStr} in cells {cellsStr} in {regionStr}";
		}
	}
}
