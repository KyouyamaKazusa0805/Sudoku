using System.Collections.Generic;
using Sudoku.Data.Meta;
using Sudoku.Drawing;

namespace Sudoku.Solving.Singles
{
	public sealed class HiddenSingleInfo : SingleInfo
	{
		public HiddenSingleInfo(
			Conclusion conclusion, ICollection<View> views, Region region,
			int digit, bool isFullHouse)
			: base(conclusion, views)
		{
			(Region, Digit, IsFullHouse) = (region, digit, isFullHouse);
		}


		public bool IsFullHouse { get; }

		public override decimal Difficulty
		{
			get
			{
				return IsFullHouse
					? 1.0m
					: Region.RegionType == RegionType.Block ? 1.2m : 1.5m;
			}
		}

		public int Digit { get; }

		public override string Name
		{
			get => IsFullHouse ? "Full House" : $"Hidden Single in {Region.RegionType}";
		}

		public Region Region { get; }


		public override string ToString()
		{
			return $"{Name}: {Digit + 1} in {Region} => {Conclusion}";
		}
	}
}
