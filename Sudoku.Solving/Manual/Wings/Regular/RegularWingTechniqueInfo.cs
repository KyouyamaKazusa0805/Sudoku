using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Wings.Regular
{
	/// <summary>
	/// Provides a usage of <b>regular wing</b> technique.
	/// </summary>
	public sealed class RegularWingTechniqueInfo : WingTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the information.
		/// </summary>
		/// <param name="conclusions">The conclusions.</param>
		/// <param name="views">The views.</param>
		/// <param name="pivot">The pivot cell offset.</param>
		/// <param name="pivotCandidatesCount">
		/// The number of candidates in pivot cell.
		/// </param>
		/// <param name="digits">The digits used.</param>
		/// <param name="cellOffsets">The cell offsets used.</param>
		public RegularWingTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int pivot, int pivotCandidatesCount, IReadOnlyList<int> digits,
			IReadOnlyList<int> cellOffsets) : base(conclusions, views) =>
			(Pivot, PivotCellCandidatesCount, Digits, CellOffsets) = (pivot, pivotCandidatesCount, digits, cellOffsets);


		/// <summary>
		/// Indicates the size of this regular wing.
		/// </summary>
		public int Size => Digits.Count;

		/// <summary>
		/// Indicates the pivot cell.
		/// </summary>
		public int Pivot { get; }

		/// <summary>
		/// Indicates the number of candidates in the pivot cell.
		/// </summary>
		public int PivotCellCandidatesCount { get; }

		/// <summary>
		/// Indicates all digits used.
		/// </summary>
		public IReadOnlyList<int> Digits { get; }

		/// <summary>
		/// Indicates all cell offsets used.
		/// </summary>
		public IReadOnlyList<int> CellOffsets { get; }

		/// <inheritdoc/>
		public override string Name
		{
			get
			{
				string[] names =
				{
					"", "", "", "", "WXYZ-Wing", "VWXYZ-Wing",
					"UVWXYZ-Wing", "TUVWXYZ-Wing", "STUVWXYZ-Wing",
					"RSTUVWXYZ-Wing"
				};
				bool isIncompleted = Size == PivotCellCandidatesCount + 1;
				return Size switch
				{
					3 => isIncompleted ? "XY-Wing" : "XYZ-Wing",
					_ when Size >= 4 && Size < 9 => isIncompleted ? $"Uncompleted {names[Size]}" : names[Size],
					_ => throw new NotSupportedException($"The specified {nameof(Size)} is out of range.")
				};
			}
		}

		/// <inheritdoc/>
		public override decimal Difficulty
		{
			get
			{
				var values = (ReadOnlySpan<decimal>)stackalloc[] { 0, 0, 0, 0, 4.6M, 4.8M, 5.1M, 5.4M, 5.7M, 6M };
				bool isIncompleted = Size == PivotCellCandidatesCount + 1;
				return Size switch
				{
					3 => isIncompleted ? 4.2M : 4.4M,
					_ when Size >= 4 && Size < 9 => isIncompleted ? values[Size] + .1M : values[Size],
					_ => throw new NotSupportedException($"The specified {nameof(Size)} is out of range.")
				};
			}
		}

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel =>
			Size switch
			{
				_ when Size >= 3 && Size < 4 => DifficultyLevel.Hard,
				_ when Size >= 4 && Size < 9 => DifficultyLevel.VeryHard,
				_ => throw new NotSupportedException($"{nameof(Size)} isn't in a valid range.")
			};

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			Size switch
			{
				2 => TechniqueCode.XyWing,
				3 => TechniqueCode.XyzWing,
				4 => TechniqueCode.WxyzWing,
				5 => TechniqueCode.VwxyzWing,
				6 => TechniqueCode.UvwxyzWing,
				7 => TechniqueCode.TuvwxyzWing,
				8 => TechniqueCode.RstuvwxyzWing,
				_ => throw Throwing.ImpossibleCase
			};


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(Digits).ToString();
			string pivotCellStr = new CellCollection(stackalloc[] { Pivot }).ToString();
			string cellOffsetsStr = new CellCollection(CellOffsets).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {digitsStr} in {pivotCellStr} with {cellOffsetsStr} => {elimStr}";
		}
	}
}
