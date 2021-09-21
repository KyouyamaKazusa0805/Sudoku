namespace Sudoku.UI.Data.ValueConverters;

/// <summary>
/// Defines a simple calculator for <see cref="CornerRadius"/>.
/// </summary>
/// <seealso cref="CornerRadius"/>
public static class CornerRadiusCalculator
{
	/// <summary>
	/// Creates a <see cref="CornerRadius"/> instance using the specified <see cref="double"/> value.
	/// </summary>
	/// <param name="cornerRadiusUniform">The uniformed corner radius width.</param>
	/// <returns>The result <see cref="CornerRadius"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CornerRadius Create(double cornerRadiusUniform) => new(cornerRadiusUniform);

	/// <summary>
	/// Gets the half-weighted <see cref="CornerRadius"/> instance.
	/// </summary>
	/// <param name="cornerRadiusUniform">
	/// The <see cref="CornerRadius"/> instance specified as a <see cref="double"/>.
	/// </param>
	/// <returns>The result <see cref="CornerRadius"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CornerRadius Half(double cornerRadiusUniform) => Divide(cornerRadiusUniform, 2);

	/// <summary>
	/// Gets the half-weighted <see cref="CornerRadius"/> instance.
	/// </summary>
	/// <param name="cornerRadius">The <see cref="CornerRadius"/> instance.</param>
	/// <returns>The result <see cref="CornerRadius"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CornerRadius Half(CornerRadius cornerRadius) => Divide(cornerRadius, new(2));

	/// <summary>
	/// Gets the result that is the result of <paramref name="dividend"/> / <paramref name="divisor"/>,
	/// both <paramref name="dividend"/> and <paramref name="divisor"/> will be treated as a
	/// <see cref="CornerRadius"/> instance.
	/// </summary>
	/// <param name="dividend">The dividend.</param>
	/// <param name="divisor">The divisor.</param>
	/// <returns>The result <see cref="CornerRadius"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CornerRadius Divide(double dividend, double divisor) =>
		Divide(new CornerRadius(dividend), new CornerRadius(divisor));

	/// <summary>
	/// Gets the result that is the result of <paramref name="dividend"/> / <paramref name="divisor"/>.
	/// </summary>
	/// <param name="dividend">The dividend.</param>
	/// <param name="divisor">The divisor.</param>
	/// <returns>The result <see cref="CornerRadius"/> instance.</returns>
	/// <exception cref="ArgumentException">Throws when four values aren't the same.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CornerRadius Divide(CornerRadius dividend, CornerRadius divisor)
	{
		var (a1, b1, c1, d1) = dividend;
		var (a2, b2, c2, d2) = divisor;

		return a1 != b1 || a1 != c1 || a1 != d1 || b1 != c1 || b1 != d1 || c1 != d1
			? throw new ArgumentException("All four values should be same.", nameof(dividend))
			: a2 != b2 || a2 != c2 || a2 != d2 || b2 != c2 || b2 != d2 || c2 != d2
				? throw new ArgumentException("All four values should be same.", nameof(divisor))
				: new(a1 / a2);
	}
}
