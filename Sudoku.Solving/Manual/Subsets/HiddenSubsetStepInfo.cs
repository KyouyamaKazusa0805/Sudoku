using System.Collections.Generic;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Globalization;
using Sudoku.Windows;

namespace Sudoku.Solving.Manual.Subsets
{
	/// <summary>
	/// Provides a usage of <b>hidden subset</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Region">The region that structure lies in.</param>
	/// <param name="Cells">All cells used.</param>
	/// <param name="Digits">All digits used.</param>
	public sealed record HiddenSubsetStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		int Region, in Cells Cells, IReadOnlyList<int> Digits)
		: SubsetStepInfo(Conclusions, Views, Region, Cells, Digits)
#if DOUBLE_LAYERED_ASSUMPTION
		, IHasParentNodeInfo
#endif
	{
		/// <inheritdoc/>
		public override decimal Difficulty => Size switch { 2 => 3.4M, 3 => 4.0M, 4 => 5.4M };

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			Size switch
			{
				2 => TechniqueCode.HiddenPair,
				3 => TechniqueCode.HiddenTriple,
				4 => TechniqueCode.HiddenQuadruple
			};


		/// <inheritdoc/>
		public override string ToString() =>
			new StringBuilder()
				.Append(Name)
				.Append(Resources.GetValue("Colon"))
				.Append(Resources.GetValue("Space"))
				.Append(new DigitCollection(Digits).ToString())
				.Append(Resources.GetValue("_HiddenSubsetSimple1"))
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
					.Append(Resources.GetValue("_HiddenSubsetSimple1"))
					.Append(new RegionCollection(Region).ToString())
					.Append(Resources.GetValue("_HiddenSubsetSimple2"))
					.Append(new DigitCollection(Digits).ToString())
					.Append(Resources.GetValue("_HiddenSubsetSimple3"))
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
				string digitsStr = new DigitCollection(Digits).ToString();
				string cellsStr = Cells.ToString();
				return new StringBuilder()
					.Append(Name)
					.Append(Resources.GetValue("Colon"))
					.Append(Resources.GetValue("_HiddenSubset1"))
					.Append(new RegionCollection(Region).ToString())
					.Append(Resources.GetValue("_HiddenSubset2"))
					.Append(digitsStr)
					.Append(Resources.GetValue("_HiddenSubset3"))
					.Append(cellsStr)
					.Append(Resources.GetValue("_HiddenSubset4"))
					.Append(cellsStr)
					.Append(Resources.GetValue("_HiddenSubset5"))
					.Append(Cells.Count)
					.Append(Resources.GetValue("_HiddenSubset6"))
					.Append(Digits.Count)
					.Append(Resources.GetValue("_HiddenSubset7"))
					.Append(digitsStr)
					.Append(Resources.GetValue("_HiddenSubset8"))
					.Append(Digits.Count)
					.Append(Resources.GetValue("_HiddenSubset9"))
					.Append(Resources.GetValue(Region.ToLabel().ToString()))
					.Append(Resources.GetValue("_HiddenSubset10"))
					.Append(new ConclusionCollection(Conclusions).ToString())
					.Append(Resources.GetValue("Period"))
					.ToString();
			}
		}

#if DOUBLE_LAYERED_ASSUMPTION
		/// <inheritdoc/>
		IEnumerable<Node> IHasParentNodeInfo.GetRuleParents(in SudokuGrid initialGrid, in SudokuGrid currentGrid)
		{
			var result = new List<Node>();
			var posMap = Cells.Empty;
			foreach (int cell in Cells)
			{
				posMap.AddAnyway(cell);
			}
			posMap = RegionMaps[Region] - posMap;

			foreach (int cell in posMap)
			{
				short initialMask = initialGrid.GetCandidateMask(cell);
				foreach (int digit in Digits)
				{
					if ((initialMask >> digit & 1) != 0)
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
