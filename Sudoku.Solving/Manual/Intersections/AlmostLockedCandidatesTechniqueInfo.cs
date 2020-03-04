using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Intersections
{
	/// <summary>
	/// Provides a usage of almost locked candidates (ALC) technique.
	/// </summary>
	public sealed class AlmostLockedCandidatesTechniqueInfo : IntersectionTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="digits">The digits.</param>
		/// <param name="baseCells">The base cells.</param>
		/// <param name="targetCells">The target cells.</param>
		/// <param name="hasValueCell">
		/// Indicates whether the structure has the value cell.
		/// </param>
		public AlmostLockedCandidatesTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			IReadOnlyList<int> digits, IReadOnlyList<int> baseCells, IReadOnlyList<int> targetCells,
			bool hasValueCell)
			: base(conclusions, views) =>
			(Digits, BaseCells, TargetCells, HasValueCell) = (digits, baseCells, targetCells, hasValueCell);


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

		/// <summary>
		/// Indicates whether the structure has a value cell.
		/// </summary>
		public bool HasValueCell { get; }

		/// <inheritdoc/>
		public override string Name => $"Almost Locked {SubsetUtils.GetNameBy(Digits.Count)}";

		/// <inheritdoc/>
		public override decimal Difficulty
		{
			get
			{
				int size = Digits.Count;
				return size switch
				{
					2 => 4.5m,
					3 => 5.2m,
					4 => 5.7m,
					_ => throw Throwing.ImpossibleCase
				} + (
					HasValueCell
						? size switch { 2 => .1m, 3 => .1m, 4 => .2m, _ => throw Throwing.ImpossibleCase }
						: 0);
			}
		}

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;


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
