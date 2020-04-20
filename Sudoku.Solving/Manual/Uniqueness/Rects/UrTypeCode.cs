namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Indicates the type code for each UR type. Some types have the another name,
	/// which have been listed also.
	/// </summary>
	/// <remarks>
	/// All types can be found in
	/// <a href="https://github.com/Sunnie-Shine/Sudoku/wiki/The-gallery-of-Unique-Rectangles">this link</a>.
	/// </remarks>
	public enum UrTypeCode : byte
	{
		/// <summary>
		/// Indicates the type 1 (UR + 1).
		/// </summary>
		[Name("Type 1")]
		Plus1 = 1,

		/// <summary>
		/// Indicates the type 1 (UR + 1).
		/// </summary>
		[Name("Type 1")]
		Type1 = 1,

		/// <summary>
		/// Indicates the type 2 (UR + 2x).
		/// </summary>
		[Name("Type 2")]
		Plus2x = 2,

		/// <summary>
		/// Indicates the type 2 (UR + 2x).
		/// </summary>
		[Name("Type 2")]
		Type2 = 2,

		/// <summary>
		/// Indicates the type 3 (UR + 2X).
		/// </summary>
		[Name("Type 3")]
		Plus2X = 3,

		/// <summary>
		/// Indicates the type 3 (UR + 2X).
		/// </summary>
		[Name("Type 3")]
		Type3 = 3,

		/// <summary>
		/// Indicates the type 4 (UR + 2X / 1SL).
		/// </summary>
		[Name("Type 4")]
		Plus2X1SL = 4,

		/// <summary>
		/// Indicates the type 4 (UR + 2X / 1SL).
		/// </summary>
		[Name("Type 4")]
		Type4 = 4,

		/// <summary>
		/// Indicates the type 5 (UR + 2d).
		/// </summary>
		[Name("Type 5")]
		Plus2d = 5,

		/// <summary>
		/// Indicates the type 5 (UR + 3x).
		/// </summary>
		[Name("Type 5")]
		Plus3x = 5,

		/// <summary>
		/// Indicates the type 5 (UR + 2d or UR + 3x).
		/// </summary>
		[Name("Type 5")]
		Type5 = 5,

		/// <summary>
		/// Indicates the type 6.
		/// </summary>
		[Name("Type 6")]
		Type6 = 6,

		/// <summary>
		/// Indicates the hidden UR (UR + 3C / 2SL).
		/// </summary>
		[Name("Hidden Unique Rectangle")]
		Hidden = 7,

		/// <summary>
		/// Indicates the hidden UR (UR + 3C / 2SL).
		/// </summary>
		[Name("Hidden Unique Rectangle")]
		Plus3C2SL = 7,

		/// <summary>
		/// Indicates the UR + 2D.
		/// </summary>
		[Name("+ 2D")]
		Plus2D = 8,

		/// <summary>
		/// Indicates the UR + 2B / 1SL.
		/// </summary>
		[Name("+ 2B / 1SL")]
		Plus2B1SL = 9,

		/// <summary>
		/// Indicates the UR + 2D / 1SL.
		/// </summary>
		[Name("+ 2D / 1SL")]
		Plus2D1SL = 10,

		/// <summary>
		/// Indicates the UR + 3X.
		/// </summary>
		[Name("+ 3X")]
		Plus3X = 11,

		/// <summary>
		/// Indicates the UR + 3x / 1SL.
		/// </summary>
		[Name("+ 3x / 1SL")]
		Plus3x1SL = 12,

		/// <summary>
		/// Indicates the UR + 3X / 1SL.
		/// </summary>
		[Name("+ 3X / 1SL")]
		Plus3X1SL = 13,

		/// <summary>
		/// Indicates the UR + 3X / 2SL.
		/// </summary>
		[Name("+ 3X / 2SL")]
		Plus3X2SL = 14,

		/// <summary>
		/// Indicates the UR + 3N / 2SL.
		/// </summary>
		[Name("+ 3N / 2SL")]
		Plus3N2SL = 15,

		/// <summary>
		/// Indicates the UR + 3U / 2SL.
		/// </summary>
		[Name("+ 3U / 2SL")]
		Plus3U2SL = 16,

		/// <summary>
		/// Indicates the UR + 3E / 2SL.
		/// </summary>
		[Name("+ 3E / 2SL")]
		Plus3E2SL = 17,

		/// <summary>
		/// Indicates the UR + 4x / 1SL.
		/// </summary>
		[Name("+ 4x / 1SL")]
		Plus4x1SL = 18,

		/// <summary>
		/// Indicates the UR + 4X / 1SL.
		/// </summary>
		[Name("+ 4X / 1SL")]
		Plus4X1SL = 19,

		/// <summary>
		/// Indicates the UR + 4x / 2SL.
		/// </summary>
		[Name("+ 4x / 2SL")]
		Plus4x2SL = 20,

		/// <summary>
		/// Indicates the UR + 4X / 2SL.
		/// </summary>
		[Name("+ 4X / 2SL")]
		Plus4X2SL = 21,

		/// <summary>
		/// Indicates the UR + 4X / 3SL.
		/// </summary>
		[Name("+ 4X / 3SL")]
		Plus4X3SL = 22,

		/// <summary>
		/// Indicates the UR + 4C / 3SL.
		/// </summary>
		[Name("+ 4C / 3SL")]
		Plus4C3SL = 23,
	}
}
