using System;
using System.Collections.Generic;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Intersections
{
	/// <summary>
	/// Provides a usage of <b>almost locked candidates</b> (ALC) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="DigitsMask">The mask to represent all digits used.</param>
	/// <param name="BaseCells">All base cells.</param>
	/// <param name="TargetCells">All target cells.</param>
	/// <param name="HasValueCell">Indicates whether the current ALC contains value cells.</param>
	public sealed record AlcTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, short DigitsMask, GridMap BaseCells,
		GridMap TargetCells, bool HasValueCell)
		: IntersectionTechniqueInfo(Conclusions, Views)
	{
		/// <summary>
		/// Indicates the size.
		/// </summary>
		public int Size => DigitsMask.PopCount();

		/// <inheritdoc/>
		public override decimal Difficulty =>
			Size switch { 2 => 4.5M, 3 => 5.2M, 4 => 5.7M, _ => throw Throwings.ImpossibleCase } +
			(HasValueCell ? Size switch { 2 => .1M, 3 => .1M, 4 => .2M, _ => throw Throwings.ImpossibleCase } : 0);

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			Size switch
			{
				2 => TechniqueCode.AlmostLockedPair,
				3 => TechniqueCode.AlmostLockedTriple,
				4 => TechniqueCode.AlmostLockedQuadruple,
				_ => throw new NotSupportedException("The current instance doesn't support.")
			};


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(DigitsMask.GetAllSets()).ToString();
			string baseCellsStr = BaseCells.ToString();
			string targetCellsStr = TargetCells.ToString();
			using var elims = new ConclusionCollection(Conclusions);
			string elimStr = elims.ToString();
			return $"{Name}: {digitsStr} in {baseCellsStr} to {targetCellsStr} => {elimStr}";
		}
	}
}
