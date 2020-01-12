using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Singles
{
	public sealed class HiddenSingleTechniqueInfo : SingleTechniqueInfo
	{
		public HiddenSingleTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int regionOffset, int cellOffset, int digit, bool enableAndIsLastDigit)
			: base(conclusions, views) =>
			(RegionOffset, CellOffset, Digit, EnableAndIsLastDigit) = (regionOffset, cellOffset, digit, enableAndIsLastDigit);


		public override string Name =>
			EnableAndIsLastDigit ? "Last Digit" : $"Hidden Single (In {RegionUtils.GetRegionName(RegionOffset)})";

		public override decimal Difficulty => EnableAndIsLastDigit ? 1.1m : RegionOffset < 9 ? 1.2m : 1.5m;

		public int RegionOffset { get; }

		public int CellOffset { get; }

		public int Digit { get; }

		public bool EnableAndIsLastDigit { get; }


		public override string ToString()
		{
			return EnableAndIsLastDigit
				? $"{Name}: {CellUtils.ToString(CellOffset)} = {Digit + 1}"
				: $"{Name}: {CellUtils.ToString(CellOffset)} = {Digit + 1} (In {RegionUtils.ToString(RegionOffset)})";
		}
	}
}
