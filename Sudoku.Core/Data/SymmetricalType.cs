namespace Sudoku.Data
{
	/// <summary>
	/// Define a symmetrical type.
	/// </summary>
	public enum SymmetricalType : byte
	{
		/// <summary>
		/// Indicates the central symmetrical type.
		/// </summary>
		[Name("Central symmetrical type")]
		Central,

		/// <summary>
		/// Indicates the diagonal symmetrical type.
		/// </summary>
		[Name("Diagonal symmetrical type")]
		Diagonal,

		/// <summary>
		/// Indicates the anti-diagonal symmetrical type.
		/// </summary>
		[Name("Anti-diagonal symmetrical type")]
		AntiDiagonal,

		/// <summary>
		/// Indicates the x-axis symmetrical type.
		/// </summary>
		[Name("X-axis symmetrical type")]
		XAxis,

		/// <summary>
		/// Indicates the y-axis symmetrical type.
		/// </summary>
		[Name("Y-axis symmetrical type")]
		YAxis,
	}
}
