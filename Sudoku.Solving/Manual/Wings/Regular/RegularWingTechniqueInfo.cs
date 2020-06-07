using System;
using System.Collections.Generic;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using static Sudoku.Solving.Constants.Processings;

namespace Sudoku.Solving.Manual.Wings.Regular
{
	/// <summary>
	/// Provides a usage of <b>regular wing</b> technique.
	/// </summary>
	public sealed class RegularWingTechniqueInfo : WingTechniqueInfo
	{
		/// <summary>
		/// The difficulty rating.
		/// </summary>
		private static readonly decimal[] DifficultyRating = { 0, 0, 0, 0, 4.6M, 4.8M, 5.1M, 5.4M, 5.7M, 6M };


		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
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
				bool isIncompleted = Size == PivotCellCandidatesCount + 1;
				switch (Size)
				{
					case 3:
					{
						return isIncompleted ? "XY-Wing" : "XYZ-Wing";
					}
					case int s when s >= 4 && s < 9:
					{
						return isIncompleted ? $"Incompleted {RegularWingNames[Size]}" : RegularWingNames[Size];
					}
					default:
					{
						throw Throwings.ImpossibleCase;
					}
				}
			}
		}

		/// <inheritdoc/>
		public override decimal Difficulty
		{
			get
			{
				bool isIncompleted = Size == PivotCellCandidatesCount + 1;
				switch (Size)
				{
					case 3:
					{
						return isIncompleted ? 4.2M : 4.4M;
					}
					case int s when s >= 4 && s < 9:
					{
						return isIncompleted ? DifficultyRating[Size] + .1M : DifficultyRating[Size];
					}
					default:
					{
						throw Throwings.ImpossibleCase;
					}
				}
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
				_ => throw Throwings.ImpossibleCase
			};


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(Digits).ToString();
			string pivotCellStr = new CellCollection(Pivot).ToString();
			string cellOffsetsStr = new CellCollection(CellOffsets).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {digitsStr} in {pivotCellStr} with {cellOffsetsStr} => {elimStr}";
		}
	}
}
