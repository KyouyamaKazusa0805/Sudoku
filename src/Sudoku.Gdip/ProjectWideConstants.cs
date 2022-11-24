namespace Sudoku.Drawing;

/// <summary>
/// Provides with project-wide constants.
/// </summary>
internal static class ProjectWideConstants
{
	/// <summary>
	/// The square root of 2.
	/// </summary>
	public const float SqrtOf2 = 1.4142135F;

	/// <summary>
	/// The rotate angle (45 degrees). This field is used for rotate the chains if some of them are overlapped.
	/// </summary>
	public const float RotateAngle = PI / 4;

	/// <summary>
	/// Indicates the default offset value.
	/// </summary>
	public const float DefaultOffset = 10F;

	/// <summary>
	/// Indicates the number of anchors hold per house.
	/// </summary>
	/// <remarks>
	/// The sudoku grid painter will draw the outlines and the inner lines, and correct the point
	/// of each digits (candidates also included). Each row or column always contains 27 candidates,
	/// so this value is 27.
	/// </remarks>
	public const int AnchorsCount = 27;
}
