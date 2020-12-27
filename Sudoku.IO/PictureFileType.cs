using NPOI.XWPF.UserModel;
using Sudoku.Solving;

namespace Sudoku.IO
{
	/// <summary>
	/// Encapsulates a picture type.
	/// </summary>
	public enum PictureFileType : byte
	{
		/// <summary>
		/// Indictaes the JPG format.
		/// </summary>
		[Alias(nameof(PictureType.JPEG))]
		Jpg,

		/// <summary>
		/// Indicates the PNG format.
		/// </summary>
		[Alias(nameof(PictureType.PNG))]
		Png,

		/// <summary>
		/// Indicates the GIF format.
		/// </summary>
		[Alias(nameof(PictureType.GIF))]
		Gif,

		/// <summary>
		/// Indicate the BMP format.
		/// </summary>
		[Alias(nameof(PictureType.BMP))]
		Bmp
	}
}
