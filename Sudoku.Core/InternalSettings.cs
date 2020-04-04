using Emgu.CV.CvEnum;

namespace Sudoku
{
	internal static class InternalSettings
	{
		public static readonly int MaxSize = 2000;

		public static readonly int RSize = 360;

		public static readonly int ThresholdMin = 50;

		public static readonly int ThresholdMax = 100;

		public static readonly bool L2Gradient = true;

		public static readonly FontFace Font = FontFace.HersheyPlain;

		public static readonly int FontSize = 3;

		public static readonly int FontSizePr = 1;

		public static readonly ChainApproxMethod ChainApprox = ChainApproxMethod.ChainApproxNone;

		public static readonly int ThOcrMin = 120;

		public static readonly int ThOcrMax = 255;
	}
}
