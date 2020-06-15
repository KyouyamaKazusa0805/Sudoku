using System;
using System.Collections.Generic;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;
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
		/// <param name="digitsMask">The digits mask.</param>
		/// <param name="cellOffsets">The cell offsets used.</param>
		public RegularWingTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int pivot, int pivotCandidatesCount, short digitsMask,
			IReadOnlyList<int> cellOffsets) : base(conclusions, views) =>
			(Pivot, PivotCellCandidatesCount, DigitsMask, CellOffsets) = (pivot, pivotCandidatesCount, digitsMask, cellOffsets);


		/// <summary>
		/// Indicates the size of this regular wing.
		/// </summary>
		public int Size => DigitsMask.CountSet();

		/// <summary>
		/// Indicates the pivot cell.
		/// </summary>
		public int Pivot { get; }

		/// <summary>
		/// Indicates the number of candidates in the pivot cell.
		/// </summary>
		public int PivotCellCandidatesCount { get; }

		/// <summary>
		/// Indicates the digits mask.
		/// </summary>
		public short DigitsMask { get; }

		/// <summary>
		/// Indicates all cell offsets used.
		/// </summary>
		public IReadOnlyList<int> CellOffsets { get; }

		/// <inheritdoc/>
		public override decimal Difficulty
		{
			get
			{
				bool isIncomplete = Size == PivotCellCandidatesCount + 1;
				switch (Size)
				{
					case 3:
					{
						return isIncomplete ? 4.2M : 4.4M;
					}
					case int s when s >= 4 && s < 9:
					{
						return isIncomplete ? DifficultyRating[Size] + .1M : DifficultyRating[Size];
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
			InternalName switch
			{
				"XY-Wing" => TechniqueCode.XyWing,
				"XYZ-Wing" => TechniqueCode.XyzWing,
				"WXYZ-Wing" => TechniqueCode.WxyzWing,
				"VWXYZ-Wing" => TechniqueCode.VwxyzWing,
				"UVWXYZ-Wing" => TechniqueCode.UvwxyzWing,
				"TUVWXYZ-Wing" => TechniqueCode.TuvwxyzWing,
				"STUVWXYZ-Wing" => TechniqueCode.StuvwxyzWing,
				"RSTUVWXYZ-Wing" => TechniqueCode.RstuvwxyzWing,
				"Incomplete WXYZ-Wing" => TechniqueCode.IncompleteWxyzWing,
				"Incomplete VWXYZ-Wing" => TechniqueCode.IncompleteVwxyzWing,
				"Incomplete UVWXYZ-Wing" => TechniqueCode.IncompleteUvwxyzWing,
				"Incomplete TUVWXYZ-Wing" => TechniqueCode.IncompleteTuvwxyzWing,
				"Incomplete STUVWXYZ-Wing" => TechniqueCode.IncompleteStuvwxyzWing,
				"Incomplete RSTUVWXYZ-Wing" => TechniqueCode.IncompleteRstuvwxyzWing,
				_ => throw Throwings.ImpossibleCase
			};

		/// <summary>
		/// Indicates the internal name.
		/// </summary>
		private string InternalName
		{
			get
			{
				bool isIncomplete = Size == PivotCellCandidatesCount + 1;
				switch (Size)
				{
					case 3:
					{
						return isIncomplete ? "XY-Wing" : "XYZ-Wing";
					}
					case int s when s >= 4 && s < 9:
					{
						return isIncomplete ? $"Incomplete {RegularWingNames[Size]}" : RegularWingNames[Size];
					}
					default:
					{
						throw Throwings.ImpossibleCase;
					}
				}
			}
		}


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(DigitsMask.GetAllSets()).ToString();
			string pivotCellStr = new CellCollection(Pivot).ToString();
			string cellOffsetsStr = new CellCollection(CellOffsets).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {digitsStr} in {pivotCellStr} with {cellOffsetsStr} => {elimStr}";
		}
	}
}
