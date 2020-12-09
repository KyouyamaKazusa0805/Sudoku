using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using System.Text;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Globalization;
using Sudoku.Windows;

namespace Sudoku.Solving.Manual.Subsets
{
	/// <summary>
	/// Provides a usage of <b>naked subset</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Region">The region that structure lies in.</param>
	/// <param name="Cells">All cells used.</param>
	/// <param name="Digits">All digits used.</param>
	/// <param name="IsLocked">Indicates whether the subset is locked.</param>
	public sealed record NakedSubsetStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		int Region, in GridMap Cells, IReadOnlyList<int> Digits, bool? IsLocked)
		: SubsetStepInfo(Conclusions, Views, Region, Cells, Digits)
#if DOUBLE_LAYERED_ASSUMPTION
		, IHasParentNodeInfo
#endif
	{
		/// <inheritdoc/>
		public override decimal Difficulty => BaseDifficulty + ExtraDifficulty;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			(IsLocked, Digits.Count) switch
			{
				(true, 2) => TechniqueCode.LockedPair,
				(false, 2) => TechniqueCode.NakedPairPlus,
				(null, 2) => TechniqueCode.NakedPair,
				(true, 3) => TechniqueCode.LockedTriple,
				(false, 3) => TechniqueCode.NakedTriplePlus,
				(null, 3) => TechniqueCode.NakedTriple,
				(false, 4) => TechniqueCode.NakedQuadruplePlus,
				(null, 4) => TechniqueCode.NakedQuadruple
			};

		/// <summary>
		/// Indicates the base difficulty.
		/// </summary>
		private decimal BaseDifficulty => Size switch { 2 => 3.0M, 3 => 3.6M, 4 => 5.0M };

		/// <summary>
		/// Indicates the extra difficulty.
		/// </summary>
		private decimal ExtraDifficulty =>
			IsLocked switch { null => 0, true => Size switch { 2 => -1.0M, 3 => -1.1M }, false => .1M };


		/// <inheritdoc/>
		public override string ToString() =>
			new StringBuilder()
				.Append(Name)
				.Append(Resources.GetValue("Colon"))
				.Append(Resources.GetValue("Space"))
				.Append(new DigitCollection(Digits).ToString())
				.Append(Resources.GetValue("_NakedSubsetSimple1"))
				.Append(new RegionCollection(Region).ToString())
				.Append(Resources.GetValue("GoesTo"))
				.Append(new ConclusionCollection(Conclusions).ToString())
				.ToString();

		/// <inheritdoc/>
		public override string ToString(CountryCode countryCode) =>
			countryCode switch
			{
				CountryCode.ZhCn =>
					new StringBuilder()
					.Append(Name)
					.Append(Resources.GetValue("Colon"))
					.Append(Resources.GetValue("_NakedSubsetSimple1"))
					.Append(new RegionCollection(Region).ToString())
					.Append(Resources.GetValue("_NakedSubsetSimple2"))
					.Append(new DigitCollection(Digits).ToString())
					.Append(Resources.GetValue("_NakedSubsetSimple3"))
					.Append(Resources.GetValue("GoesTo"))
					.Append(new ConclusionCollection(Conclusions).ToString())
					.ToString(),
				_ => base.ToString(countryCode)
			};

		/// <inheritdoc/>
		public override string ToFullString(CountryCode countryCode)
		{
			return countryCode switch
			{
				CountryCode.ZhCn => toChinese(),
				_ => base.ToFullString(countryCode)
			};

			string toChinese()
			{
				string regionStr = new RegionCollection(Region).ToString();
				string digitsStr = new DigitCollection(Digits).ToString();
				string cellsStr = new GridMap(Cells).ToString();
				return new StringBuilder()
					.Append(Name)
					.Append(Resources.GetValue("Colon"))
					.Append(Resources.GetValue("_NakedSubset1"))
					.Append(regionStr)
					.Append(Resources.GetValue("_NakedSubset2"))
					.Append(cellsStr)
					.Append(Resources.GetValue("_NakedSubset3"))
					.Append(digitsStr)
					.Append(Resources.GetValue("_NakedSubset4"))
					.Append(regionStr)
					.Append(Resources.GetValue("_NakedSubset5"))
					.Append(cellsStr)
					.Append(Resources.GetValue("_NakedSubset6"))
					.Append(Cells.Count)
					.Append(Resources.GetValue("_NakedSubset7"))
					.Append(digitsStr)
					.Append(Resources.GetValue("_NakedSubset8"))
					.Append(Digits.Count)
					.Append(Resources.GetValue("_NakedSubset9"))
					.AppendJoin(Resources.GetValue("_NakedSubset10"), from digit in Digits select digit + 1)
					.Append(Resources.GetValue("_NakedSubset11"))
					.Append(Resources.GetValue(Processings.GetLabel(Region).ToString()))
					.Append(Resources.GetValue("_NakedSubset12"))
					.Append(digitsStr)
					.Append(Resources.GetValue("_NakedSubset13"))
					.Append(regionStr)
					.Append(Resources.GetValue("_NakedSubset14"))
					.Append(new ConclusionCollection(Conclusions).ToString())
					.Append(Resources.GetValue("Period"))
					.ToString();
			}
		}

#if DOUBLE_LAYERED_ASSUMPTION
		/// <inheritdoc/>
		IEnumerable<Node> IHasParentNodeInfo.GetRuleParents(in SudokuGrid initialGrid, in SudokuGrid currentGrid)
		{
			short digitsMask = 0;
			foreach (int digit in Digits)
			{
				digitsMask |= (short)(1 << digit);
			}
			digitsMask = (short)(SudokuGrid.MaxCandidatesMask & ~digitsMask);

			var result = new List<Node>();
			foreach (int digit in digitsMask)
			{
				foreach (int cell in Cells)
				{
					var mask = initialGrid.GetCandidateMask(cell);
					if ((mask >> digit & 1) != 0)
					{
						result.Add(new(cell, digit, false));
					}
				}
			}

			return result;
		}
#endif
	}
}
