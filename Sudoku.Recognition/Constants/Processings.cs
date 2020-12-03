#if SUDOKU_RECOGNITION

using Emgu.CV.CvEnum;

namespace Sudoku.Recognition.Constants
{
	/// <summary>
	/// Define the internal settings.
	/// </summary>
	internal static class Processings
	{
		/// <summary>
		/// Indicates the maximum size.
		/// </summary>
		public const int MaxSize = 2000;

		/// <summary>
		/// Indicates the R-size.
		/// </summary>
		public const int RSize = 360;

		/// <summary>
		/// Indicates the minimum threshold.
		/// </summary>
		public const int ThresholdMin = 50;

		/// <summary>
		/// Indicates the maximum threshold.
		/// </summary>
		public const int ThresholdMax = 100;

		/// <summary>
		/// Indicates the L2Gradient.
		/// </summary>
		public const bool L2Gradient = true;

		/// <summary>
		/// Indicates the font.
		/// </summary>
		public const FontFace Font = FontFace.HersheyPlain;

		/// <summary>
		/// Indicates the font size.
		/// </summary>
		public const int FontSize = 3;

		/// <summary>
		/// Indicates the font size pr.
		/// </summary>
		public const int FontSizePr = 1;

		/// <summary>
		/// Indicates the ChainApprox.
		/// </summary>
		public const ChainApproxMethod ChainApprox = ChainApproxMethod.ChainApproxNone;

		/// <summary>
		/// Indicates the ThOcrMin.
		/// </summary>
		public const int ThOcrMin = 120;

		/// <summary>
		/// Indicates the ThOcrMax.
		/// </summary>
		public const int ThOcrMax = 255;
	}
}
#endif