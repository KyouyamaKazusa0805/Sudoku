namespace Sudoku.Data.Meta
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
		/// Indicates the diagnoal symmetrical type.
		/// </summary>
		[Name("Diagonal symmetrical type")]
		Diagonal,
		/// <summary>
		/// Indicates the anti-diagonal symmetrical type.
		/// </summary>
		[Name("Anti-diagnoal symmetrical type")]
		AntiDiagonal,
	}
}
