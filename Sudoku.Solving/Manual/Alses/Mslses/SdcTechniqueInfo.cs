using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Alses.Mslses
{
	/// <summary>
	/// Provides a usage of <b>sue de coq</b> (SdC) technique.
	/// </summary>
	public sealed class SdcTechniqueInfo : MslsTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="block">The block.</param>
		/// <param name="line">The line.</param>
		/// <param name="blockMask">The block mask.</param>
		/// <param name="lineMask">The line mask.</param>
		/// <param name="intersectionMask">The intersection mask.</param>
		/// <param name="isCannibalistic">Indicates whether the SdC is cannibalistic.</param>
		/// <param name="isolatedDigitsMask">The isolated digits mask.</param>
		/// <param name="blockCells">The map of block cells.</param>
		/// <param name="lineCells">The map of line cells.</param>
		/// <param name="intersectionCells">The map of intersection cells.</param>
		public SdcTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int block, int line, short blockMask, short lineMask, short intersectionMask,
			bool isCannibalistic, short isolatedDigitsMask, GridMap blockCells, GridMap lineCells,
			GridMap intersectionCells)
			: base(conclusions, views)
		{
			Block = block;
			Line = line;
			BlockMask = blockMask;
			LineMask = lineMask;
			IntersectionMask = intersectionMask;
			IsCannibalistic = isCannibalistic;
			IsolatedDigitsMask = isolatedDigitsMask;
			BlockCells = blockCells;
			LineCells = lineCells;
			IntersectionCells = intersectionCells;
		}


		/// <summary>
		/// Indicates the block.
		/// </summary>
		public int Block { get; }

		/// <summary>
		/// Indicates the line.
		/// </summary>
		public int Line { get; }

		/// <summary>
		/// Indicates the block mask.
		/// </summary>
		public short BlockMask { get; }

		/// <summary>
		/// Indicates the line mask.
		/// </summary>
		public short LineMask { get; }

		/// <summary>
		/// Indicates the intersection mask.
		/// </summary>
		public short IntersectionMask { get; }

		/// <summary>
		/// Indicates whether the specified SdC is cannibalistic.
		/// </summary>
		public bool IsCannibalistic { get; }

		/// <summary>
		/// Indicates the isolated digit mask.
		/// </summary>
		public short IsolatedDigitsMask { get; }

		/// <summary>
		/// Indicates the block cells.
		/// </summary>
		public GridMap BlockCells { get; }

		/// <summary>
		/// Indicates the line cells.
		/// </summary>
		public GridMap LineCells { get; }

		/// <summary>
		/// Indicates the intersection cells.
		/// </summary>
		public GridMap IntersectionCells { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => 5M + (IsolatedDigitsMask != 0 ? .1M : 0) + (IsCannibalistic ? .2M : 0);

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			IsCannibalistic ? TechniqueCode.CannibalizedSdc : TechniqueCode.Sdc;


		/// <inheritdoc/>
		public override string ToString()
		{
			string blockCellsStr = new CellCollection(BlockCells).ToString();
			string blockDigitsStr = new DigitCollection(BlockMask.GetAllSets()).ToString(null);
			string lineCellsStr = new CellCollection(LineCells).ToString();
			string lineDigitsStr = new DigitCollection(LineMask.GetAllSets()).ToString(null);
			string interCellsStr = new CellCollection(IntersectionCells).ToString();
			string interDigitsStr = new DigitCollection(IntersectionMask.GetAllSets()).ToString(null);
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: {interCellsStr}({interDigitsStr}) - " +
				$"{blockCellsStr}({blockDigitsStr}) & {lineCellsStr}({lineDigitsStr}) => {elimStr}";
		}
	}
}
