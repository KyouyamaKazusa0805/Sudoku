using Emgu.CV.CvEnum;

namespace Sudoku.Recognition;

/// <summary>
/// Define the project-wide constants.
/// </summary>
internal static class ProjectWideConstants
{
	/// <summary>
	/// Indicates the L2Gradient.
	/// </summary>
	public const bool L2Gradient = true;

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
	/// Indicates the font size.
	/// </summary>
	public const int FontSize = 3;

	/// <summary>
	/// Indicates the font size pr.
	/// </summary>
	public const int FontSizePr = 1;

	/// <summary>
	/// Indicates the ThOcrMin.
	/// </summary>
	public const int ThOcrMin = 120;

	/// <summary>
	/// Indicates the ThOcrMax.
	/// </summary>
	public const int ThOcrMax = 255;

	/// <summary>
	/// Indicates the font face.
	/// </summary>
	public const FontFace Font = FontFace.HersheyPlain;

	/// <summary>
	/// Indicates the behavior that specifies and executes the chain approximation algorithm.
	/// </summary>
	public const ChainApproxMethod ChainApproximation = ChainApproxMethod.ChainApproxNone;
}
