using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>unique rectangle</b> (UR) or
	/// <b>avoidable rectangle</b> (AR) with wings technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="TechniqueCode2">The technique code.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="IsAvoidable">Indicates whether the structure is an AR.</param>
	/// <param name="ExtraCells">The extra cells.</param>
	/// <param name="ExtraDigits">The extra digits.</param>
	/// <param name="Pivots">The pivot cells.</param>
	/// <param name="AbsoluteOffset">The absolute offset that used in sorting.</param>
	public sealed record UrWithWingStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		TechniqueCode TechniqueCode2, int Digit1, int Digit2, int[] Cells, bool IsAvoidable,
		IEnumerable<int> ExtraCells, IEnumerable<int> ExtraDigits, IEnumerable<int> Pivots, int AbsoluteOffset)
		: UrStepInfo(Conclusions, Views, TechniqueCode2, Digit1, Digit2, Cells, IsAvoidable, AbsoluteOffset)
	{
		/// <summary>
		/// Indicates the difficulty rating extra.
		/// </summary>
		private static readonly decimal[] ExtraDifficulty = { .2M, .3M, .5M };


		/// <inheritdoc/>
		public override decimal Difficulty => 4.4M + AvoidableExtraDifficulty + WingSizeExtraDifficulty;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <summary>
		/// Indicates the extra difficulty on whether the UR is an AR.
		/// </summary>
		private decimal AvoidableExtraDifficulty => IsAvoidable ? .1M : 0;

		/// <summary>
		/// Indicates the extra difficulty on wing size.
		/// </summary>
		private decimal WingSizeExtraDifficulty =>
			ExtraDifficulty[TechniqueCode switch
			{
				TechniqueCode.UrXyWing or TechniqueCode.ArXyWing => 0,
				TechniqueCode.UrXyzWing or TechniqueCode.ArXyzWing => 1,
				TechniqueCode.UrWxyzWing or TechniqueCode.ArWxyzWing => 2
			}];


		/// <inheritdoc/>
		public override string ToString() => base.ToString();

		/// <inheritdoc/>
		protected override string? GetAdditional()
		{
			string pivotsStr = new Cells(Pivots).ToString();
			string digitsStr = new DigitCollection(ExtraDigits).ToString();
			string cellsStr = new Cells(ExtraCells).ToString();
			return $"pivots: {pivotsStr}, with digits: {digitsStr} in cells {cellsStr}";
		}
	}
}
