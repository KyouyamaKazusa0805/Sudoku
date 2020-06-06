namespace Sudoku.Constants
{
	/// <summary>
	/// Indicates the region label.
	/// </summary>
	public enum RegionLabel : sbyte
	{
		/// <summary>
		/// Indicates the lower limit.
		/// </summary>
		LowerLimit = -1,

		/// <summary>
		/// Indicates the block.
		/// </summary>
		Block,

		/// <summary>
		/// Indicates the row.
		/// </summary>
		Row,

		/// <summary>
		/// Indicates the column.
		/// </summary>
		Column,

		/// <summary>
		/// Indicates the upper limit.
		/// </summary>
		UpperLimit = Column + 1,
	}
}
