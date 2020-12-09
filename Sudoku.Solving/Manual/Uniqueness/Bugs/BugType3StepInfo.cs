using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using static Sudoku.Solving.Constants.Processings;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of <b>bivalue universal grave</b> (BUG) type 3 technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="TrueCandidates">All true candidates.</param>
	/// <param name="Digits">All digits.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="IsNaked">Indicates whether the subset is naked.</param>
	public sealed record BugType3StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		IReadOnlyList<int> TrueCandidates, IReadOnlyList<int> Digits,
		IReadOnlyList<int> Cells, bool IsNaked) : UniquenessStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.6M + Digits.Count * .1M + (IsNaked ? 0 : .1M);

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.BugType3;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(Digits).ToString();
			string cellsStr = new GridMap(Cells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string sizeStr = SubsetNames[Digits.Count].ToLower();
			string trueCandidatesStr = new SudokuMap(TrueCandidates).ToString();
			string subsetTypeStr = IsNaked ? "naked" : "hidden";
			return
				$"{Name}: True candidates {trueCandidatesStr} with {subsetTypeStr} {sizeStr} {digitsStr} " +
				$"in cells {cellsStr} => {elimStr}";
		}
	}
}
