namespace Sudoku.UI.Data.ValueConverters;

/// <summary>
/// Defines a simple calculator for <see cref="double"/>.
/// </summary>
/// <seealso cref="double"/>
internal static class DoubleCalculator
{
	/// <summary>
	/// Make the division.
	/// </summary>
	/// <param name="dividend">The dividend.</param>
	/// <param name="divisor">The divisor.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Divide(double dividend, double divisor) => dividend / divisor;
}
