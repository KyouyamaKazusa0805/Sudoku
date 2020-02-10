using System;
using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Intersections
{
	/// <summary>
	/// Provides a usage of almost locked candidates (ALC) technique.
	/// </summary>
	public sealed class AlmostLockedCandidatesTechniqueInfo : TechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="digits">The digits.</param>
		/// <param name="baseCells">The base cells.</param>
		/// <param name="targetCells">The target cells.</param>
		public AlmostLockedCandidatesTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			IReadOnlyList<int> digits, IReadOnlyList<int> baseCells, IReadOnlyList<int> targetCells)
			: base(conclusions, views) =>
			(Digits, BaseCells, TargetCells) = (digits, baseCells, targetCells);


		/// <summary>
		/// Indicates the digits the technique used.
		/// </summary>
		public IReadOnlyList<int> Digits { get; }

		/// <summary>
		/// Indicates the base cells.
		/// </summary>
		public IReadOnlyList<int> BaseCells { get; }

		/// <summary>
		/// Indicates the target cells.
		/// </summary>
		public IReadOnlyList<int> TargetCells { get; }

		/// <inheritdoc/>
		public override string Name => $"Almost Locked {SubsetUtils.GetNameBy(Digits.Count)}";

		/// <inheritdoc/>
		public override decimal Difficulty
		{
			get
			{
				return Digits.Count switch
				{
					2 => 4.5m,
					3 => 5.2m,
					4 => 5.5m,
					_ => throw new NotSupportedException($"The specified Size is out of valid range.")
				};
			}
		}

		/// <inheritdoc/>
		public override DifficultyLevels DifficultyLevel => DifficultyLevels.Hard;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = DigitCollection.ToString(Digits);
			string baseCellsStr = CellCollection.ToString(BaseCells);
			string targetCellsStr = CellCollection.ToString(TargetCells);
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: {digitsStr} in {baseCellsStr} to {targetCellsStr} => {elimStr}";
		}
	}
}
