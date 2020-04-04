#if SUDOKU_RECOGNIZING
using System.Drawing;
using Sudoku.Data;

namespace Sudoku.Recognizations
{
	/// <summary>
	/// Define a grid that can be recognized.
	/// </summary>
	public static class RecognizationUtils
	{
		/// <summary>
		/// Recognize the image.
		/// </summary>
		/// <param name="image">The image.</param>
		/// <returns>The grid.</returns>
		public static IReadOnlyGrid Recorgnize(Bitmap image) =>
			CellRecognizer.RecognizeDigits(new GridRecognizer(image).Recognize(), out _);
	}
}
#endif