using System;

namespace Sudoku.Data
{
	/// <summary>
	/// Define a symmetry type.
	/// </summary>
	[Flags]
	/*closed*/
	public enum SymmetryType : byte
	{
		/// <summary>
		/// Indicates none of symmetry type.
		/// </summary>
		[Name("No symmetry")]
		None = 0,

		/// <summary>
		/// Indicates the central symmetry type.
		/// </summary>
		[Name("Central symmetry type")]
		Central = 1,

		/// <summary>
		/// Indicates the diagonal symmetry type.
		/// </summary>
		[Name("Diagonal symmetry type")]
		Diagonal = 2,

		/// <summary>
		/// Indicates the anti-diagonal symmetry type.
		/// </summary>
		[Name("Anti-diagonal symmetry type")]
		AntiDiagonal = 4,

		/// <summary>
		/// Indicates the x-axis symmetry type.
		/// </summary>
		[Name("X-axis symmetry type")]
		XAxis = 8,

		/// <summary>
		/// Indicates the y-axis symmetry type.
		/// </summary>
		[Name("Y-axis symmetry type")]
		YAxis = 16,

		/// <summary>
		/// Indicates both X-axis and Y-axis symmetry types.
		/// </summary>
		[Name("Both X-axis and Y-axis")]
		AxisBoth = 32,

		/// <summary>
		/// Indicates both diagonal and anti-diagonal symmetry types.
		/// </summary>
		[Name("Both diagonal and anti-diagonal")]
		DiagonalBoth = 64,

		/// <summary>
		/// Indicates all symmetry types should be satisfied.
		/// </summary>
		[Name("All symmetry type")]
		All = 128,
	}
}
