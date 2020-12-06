using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using static Sudoku.Solving.Constants.Processings;

namespace Sudoku.Solving.Manual.Wings.Regular
{
	/// <summary>
	/// Provides a usage of <b>regular wing</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Pivot">The pivot cell.</param>
	/// <param name="PivotCandidatesCount">The number of candidates that is in the pivot.</param>
	/// <param name="DigitsMask">The mask of all digits used.</param>
	/// <param name="Cells">The cells used.</param>
	public sealed record RegularWingTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Pivot,
		int PivotCandidatesCount, short DigitsMask, IReadOnlyList<int> Cells)
		: WingTechniqueInfo(Conclusions, Views)
	{
		/// <summary>
		/// The difficulty rating.
		/// </summary>
		private static readonly decimal[] DifficultyRating = { 0, 0, 0, 0, 4.6M, 4.8M, 5.1M, 5.4M, 5.7M, 6.0M };


		/// <summary>
		/// Indicates whether the structure is incomplete.
		/// </summary>
		public bool IsIncomplete => Size == PivotCandidatesCount + 1;

		/// <summary>
		/// Indicates the size of this regular wing.
		/// </summary>
		public int Size => DigitsMask.PopCount();

		/// <inheritdoc/>
		public override decimal Difficulty =>
			Size switch
			{
				3 => IsIncomplete ? 4.2M : 4.4M,
				>= 4 and < 9 => IsIncomplete ? DifficultyRating[Size] + .1M : DifficultyRating[Size]
			};

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel =>
			Size switch
			{
				>= 3 and <= 4 => DifficultyLevel.Hard,
				> 4 and < 9 => DifficultyLevel.Fiendish
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
				"Incomplete RSTUVWXYZ-Wing" => TechniqueCode.IncompleteRstuvwxyzWing
			};

		/// <summary>
		/// Indicates the internal name.
		/// </summary>
		private string InternalName =>
			Size switch
			{
				3 => IsIncomplete ? "XY-Wing" : "XYZ-Wing",
				>= 4 and < 9 => IsIncomplete ? $"Incomplete {RegularWingNames[Size]}" : RegularWingNames[Size]
			};


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(DigitsMask.GetAllSets()).ToString();
			string pivotCellStr = new GridMap { Pivot }.ToString();
			string cellOffsetsStr = new GridMap(Cells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {digitsStr} in {pivotCellStr} with {cellOffsetsStr} => {elimStr}";
		}
	}
}
