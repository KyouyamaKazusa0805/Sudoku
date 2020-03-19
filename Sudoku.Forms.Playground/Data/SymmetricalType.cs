using System;

namespace Sudoku.Data
{
	/// <summary>
	/// Define a symmetrical type.
	/// </summary>
	[Flags]
	public enum SymmetricalType : byte
	{
		/// <summary>
		/// Indicates none of symmetrical type.
		/// </summary>
		[Name("The pattern does not have any symmetrical type")]
		None = 0,

		/// <summary>
		/// Indicates the central symmetrical type.
		/// </summary>
		[Name("Central symmetrical type")]
		Central = 1,

		/// <summary>
		/// Indicates the diagonal symmetrical type.
		/// </summary>
		[Name("Diagonal symmetrical type")]
		Diagonal = 2,

		/// <summary>
		/// Indicates the anti-diagonal symmetrical type.
		/// </summary>
		[Name("Anti-diagonal symmetrical type")]
		AntiDiagonal = 4,

		/// <summary>
		/// Indicates the x-axis symmetrical type.
		/// </summary>
		[Name("X-axis symmetrical type")]
		XAxis = 8,

		/// <summary>
		/// Indicates the y-axis symmetrical type.
		/// </summary>
		[Name("Y-axis symmetrical type")]
		YAxis = 16,

		/// <summary>
		/// Indicates both X-axis and Y-axis type.
		/// </summary>
		[Name("Both X-axis and Y-axis")]
		AxisBoth = 32,

		/// <summary>
		/// Indicates both diagonal and anti-diagonal type.
		/// </summary>
		[Name("Both diagonal and anti-diagonal")]
		DiagonalBoth = 64,

		/// <summary>
		/// Indicates all symmetrical types should be satisfied.
		/// </summary>
		[Name("All symmetrical type")]
		All = 128,
	}
}
