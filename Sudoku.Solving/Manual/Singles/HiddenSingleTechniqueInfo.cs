using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.Singles
{
	/// <summary>
	/// Indicates a using of <b>hidden single</b> technique.
	/// </summary>
	public sealed class HiddenSingleTechniqueInfo : SingleTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with information.
		/// </summary>
		/// <param name="conclusions">The conclusions.</param>
		/// <param name="views">The views.</param>
		/// <param name="regionOffset">The region offset.</param>
		/// <param name="cellOffset">The cell offset.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="enableAndIsLastDigit">
		/// Indicates whether the solver enables last digit.
		/// </param>
		public HiddenSingleTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int regionOffset, int cellOffset, int digit, bool enableAndIsLastDigit)
			: base(conclusions, views, cellOffset, digit) =>
			(RegionOffset, EnableAndIsLastDigit) = (regionOffset, enableAndIsLastDigit);


		/// <summary>
		/// Indicates the region offset.
		/// </summary>
		public int RegionOffset { get; }

		/// <summary>
		/// Indicates whether the solver enables last digit technique.
		/// </summary>
		public bool EnableAndIsLastDigit { get; }

		/// <inheritdoc/>
		public override string Name =>
			EnableAndIsLastDigit ? "Last Digit" : $"Hidden Single (In {GetLabel(RegionOffset)})";

		/// <inheritdoc/>
		public override decimal Difficulty => EnableAndIsLastDigit ? 1.1M : RegionOffset < 9 ? 1.2M : 1.5M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			EnableAndIsLastDigit ? TechniqueCode.LastDigit : TechniqueCode.HiddenSingle;


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellStr = new CellCollection(Cell).ToString();
			string regionStr = new RegionCollection(RegionOffset).ToString();
			int value = Digit + 1;
			return EnableAndIsLastDigit
				? $"{Name}: {cellStr} = {value}"
				: $"{Name}: {cellStr} = {value} in {regionStr}";
		}
	}
}
