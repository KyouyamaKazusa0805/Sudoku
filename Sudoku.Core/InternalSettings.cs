#if SUDOKU_RECOGNIZING
using Emgu.CV.CvEnum;

namespace Sudoku
{
	/// <summary>
	/// Define the internal settings.
	/// </summary>
	internal static class InternalSettings
	{
		/// <summary>
		/// Indicates the maximum size.
		/// </summary>
		public static readonly int MaxSize = 2000;

		/// <summary>
		/// Indicates the R-size.
		/// </summary>
		public static readonly int RSize = 360;

		/// <summary>
		/// Indicates the minimum threshold.
		/// </summary>
		public static readonly int ThresholdMin = 50;

		/// <summary>
		/// Indicates the maximum threshold.
		/// </summary>
		public static readonly int ThresholdMax = 100;

		/// <summary>
		/// Indicates the L2Gradient.
		/// </summary>
		public static readonly bool L2Gradient = true;

		/// <summary>
		/// Indicates the font.
		/// </summary>
		public static readonly FontFace Font = FontFace.HersheyPlain;

		/// <summary>
		/// Indicates the font size.
		/// </summary>
		public static readonly int FontSize = 3;

		/// <summary>
		/// Indicates the font size pr.
		/// </summary>
		public static readonly int FontSizePr = 1;

		/// <summary>
		/// Indicates the ChainApprox.
		/// </summary>
		public static readonly ChainApproxMethod ChainApprox = ChainApproxMethod.ChainApproxNone;

		/// <summary>
		/// Indicates the ThOcrMin.
		/// </summary>
		public static readonly int ThOcrMin = 120;

		/// <summary>
		/// Indicates the ThOcrMax.
		/// </summary>
		public static readonly int ThOcrMax = 255;
	}
}
#endif