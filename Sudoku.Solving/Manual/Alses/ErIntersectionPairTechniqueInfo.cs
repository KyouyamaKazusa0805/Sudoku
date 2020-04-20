using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Provides a usage of <b>empty rectangle intersection pair</b> technique.
	/// </summary>
	public sealed class ErIntersectionPairTechniqueInfo : AlsTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="startCell">The start cell.</param>
		/// <param name="endCell">The end cell.</param>
		/// <param name="region">The region that empty rectangle forms.</param>
		/// <param name="digit1">The digit 1.</param>
		/// <param name="digit2">The digit 2.</param>
		public ErIntersectionPairTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int startCell, int endCell, int region, int digit1, int digit2) : base(conclusions, views) =>
			(StartCell, EndCell, Region, Digit1, Digit2) = (startCell, endCell, region, digit1, digit2);


		/// <summary>
		/// Indicates the region that empty rectangle forms.
		/// </summary>
		public int Region { get; }

		/// <summary>
		/// Indicates the start cell.
		/// </summary>
		public int StartCell { get; }

		/// <summary>
		/// Indicates the end cell.
		/// </summary>
		public int EndCell { get; }

		/// <summary>
		/// Indicates the digit 1.
		/// </summary>
		public int Digit1 { get; }

		/// <summary>
		/// Indicates the digit 2.
		/// </summary>
		public int Digit2 { get; }

		/// <inheritdoc/>
		public override string Name => "Empty Rectangle Intersection Pair";

		/// <inheritdoc/>
		public override decimal Difficulty => 6M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;


		/// <inheritdoc/>
		public override string ToString()
		{
			int d1 = Digit1 + 1;
			int d2 = Digit2 + 1;
			string sCellStr = CellUtils.ToString(StartCell);
			string eCellStr = CellUtils.ToString(EndCell);
			string elimStr = ConclusionCollection.ToString(Conclusions);
			string regionStr = RegionUtils.ToString(Region);
			return
				$"{Name}: Digits {d1}, {d2} in bivalue cells {sCellStr} and {eCellStr} " +
				$"with empty rectangle in {regionStr} => {elimStr}";
		}
	}
}
