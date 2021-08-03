#if SOLUTION_WIDE_CODE_ANALYSIS
using System.Diagnostics.CodeAnalysis;
#endif

namespace Sudoku.IO
{
	/// <summary>
	/// Encapsulates a picture type.
	/// </summary>
#if SOLUTION_WIDE_CODE_ANALYSIS
	[Closed]
#endif
	public enum PictureFileType : byte
	{
		/// <summary>
		/// Indicates the JPG format.
		/// </summary>
		Jpg,

		/// <summary>
		/// Indicates the PNG format.
		/// </summary>
		Png,

		/// <summary>
		/// Indicates the GIF format.
		/// </summary>
		Gif,

		/// <summary>
		/// Indicate the BMP format.
		/// </summary>
		Bmp
	}
}
