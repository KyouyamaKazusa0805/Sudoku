using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Alses.Mslses
{
	/// <summary>
	/// Provides a usage of <b>3-dimension sue de coq</b> technique.
	/// </summary>
	public sealed class Sdc3dTechniqueInfo : MslsTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="rowDigitsMask">The row digits mask.</param>
		/// <param name="columnDigitsMask">The column digits mask.</param>
		/// <param name="blockDigitsMask">The block digits mask.</param>
		/// <param name="rowCells">The row cells map.</param>
		/// <param name="columnCells">The column cells map.</param>
		/// <param name="blockCells">The block cells map.</param>
		public Sdc3dTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, short rowDigitsMask,
			short columnDigitsMask, short blockDigitsMask, GridMap rowCells, GridMap columnCells,
			GridMap blockCells) : base(conclusions, views) =>
			(RowDigitsMask, ColumnDigitsMask, BlockDigitsMask, RowCells, ColumnCells, BlockCells) =
			(rowDigitsMask, columnDigitsMask, blockDigitsMask, rowCells, columnCells, blockCells);


		/// <summary>
		/// Indicates the row digits mask.
		/// </summary>
		public short RowDigitsMask { get; }

		/// <summary>
		/// Indicates the column digits mask.
		/// </summary>
		public short ColumnDigitsMask { get; }

		/// <summary>
		/// Indicates the block digits mask.
		/// </summary>
		public short BlockDigitsMask { get; }

		/// <summary>
		/// Indicates the row cells map.
		/// </summary>
		public GridMap RowCells { get; }

		/// <summary>
		/// Indicates the column cells map.
		/// </summary>
		public GridMap ColumnCells { get; }

		/// <summary>
		/// Indicates the block cells map.
		/// </summary>
		public GridMap BlockCells { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => 5.5M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.Sdc3d;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;


		/// <inheritdoc/>
		public override string ToString()
		{
			string cells1Str = new CellCollection(RowCells).ToString();
			string cells2Str = new CellCollection(ColumnCells).ToString();
			string cells3Str = new CellCollection(BlockCells).ToString();
			string digits1Str = new DigitCollection(RowDigitsMask.GetAllSets()).ToString();
			string digits2Str = new DigitCollection(ColumnDigitsMask.GetAllSets()).ToString();
			string digits3Str = new DigitCollection(BlockDigitsMask.GetAllSets()).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: {cells1Str}({digits1Str}) + {cells2Str}({digits2Str}) + " +
				$"{cells3Str}({digits3Str}) => {elimStr}";
		}
	}
}
