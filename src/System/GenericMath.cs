namespace System;

/// <summary>
/// Provides with methods for math operations, same as types <see cref="Math"/> and <see cref="MathF"/>,
/// but uses generic.
/// </summary>
/// <seealso cref="Math"/>
/// <seealso cref="MathF"/>
[RequiresPreviewFeatures]
public static class GenericMath
{
	/// <summary>
	/// Returns the absolute value of a value.
	/// </summary>
	/// <typeparam name="T">The type of the number.</typeparam>
	/// <param name="value">The value.</param>
	/// <returns>The absolute result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Abs<T>(T value) where T : INumber<T> => T.Abs(value);

	/// <summary>
	/// Produces the quotient and the remainder of two numbers.
	/// </summary>
	/// <typeparam name="T">The type of the number.</typeparam>
	/// <param name="dividend">The dividend.</param>
	/// <param name="divisor">The divisor.</param>
	/// <returns>The quotient and the remainder of the specified numbers.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (T Quotient, T Remainder) DivRem<T>(T dividend, T divisor) where T : INumber<T> =>
		T.DivRem(dividend, divisor);

	/// <summary>
	/// Clamps the specified value. If the value is less than <paramref name="min"/>,
	/// the return value will be the same as <paramref name="min"/>; if the value is greater than
	/// <paramref name="max"/>, the return value will be the same as <paramref name="max"/>; else
	/// the <paramref name="value"/> itself. The details are displayed
	/// in the <see href="#returns">returns</see> part.
	/// </summary>
	/// <typeparam name="T">The type of the number.</typeparam>
	/// <param name="value">The value to check.</param>
	/// <param name="min">The minimum number.</param>
	/// <param name="max">The maximum number.</param>
	/// <returns>
	/// The result. Returns:
	/// <list type="table">
	/// <item>
	/// <term>The value same as <paramref name="min"/></term>
	/// <description>If the <paramref name="value"/> is less than <paramref name="min"/>.</description>
	/// </item>
	/// <item>
	/// <term>The value same as <paramref name="max"/></term>
	/// <description>If the <paramref name="value"/> is greater than <paramref name="max"/>.</description>
	/// </item>
	/// <item>
	/// <term>The value same as <paramref name="value"/></term>
	/// <description>Other cases.</description>
	/// </item>
	/// </list>
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Clamp<T>(T value, T min, T max) where T : INumber<T> => T.Clamp(value, min, max);

	/// <summary>
	/// Get the maximum value between <paramref name="val1"/> and <paramref name="val2"/>.
	/// </summary>
	/// <typeparam name="T">The type of the value to check.</typeparam>
	/// <param name="val1">The first value to check.</param>
	/// <param name="val2">The second value to check.</param>
	/// <returns>The result value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Max<T>(T val1, T val2) where T : INumber<T> => T.Max(val1, val2);

	/// <summary>
	/// Get the minimum value between <paramref name="val1"/> and <paramref name="val2"/>.
	/// </summary>
	/// <typeparam name="T">The type of the value to check.</typeparam>
	/// <param name="val1">The first value to check.</param>
	/// <param name="val2">The second value to check.</param>
	/// <returns>The result value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Min<T>(T val1, T val2) where T : INumber<T> => T.Min(val1, val2);

	/// <summary>
	/// Get the sign of the value.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="value">The value.</param>
	/// <returns>The sign value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Sign<T>(T value) where T : INumber<T> => T.Sign(value);

#pragma warning disable CS1591
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Cos<T>(T value) where T : IFloatingPoint<T> => T.Cos(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Cosh<T>(T value) where T : IFloatingPoint<T> => T.Cosh(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Sin<T>(T value) where T : IFloatingPoint<T> => T.Sin(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Sinh<T>(T value) where T : IFloatingPoint<T> => T.Sinh(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Tan<T>(T value) where T : IFloatingPoint<T> => T.Tan(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Tanh<T>(T value) where T : IFloatingPoint<T> => T.Tanh(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Acos<T>(T value) where T : IFloatingPoint<T> => T.Acos(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Acosh<T>(T value) where T : IFloatingPoint<T> => T.Acosh(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Asin<T>(T value) where T : IFloatingPoint<T> => T.Asin(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Asinh<T>(T value) where T : IFloatingPoint<T> => T.Asinh(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Atan<T>(T value) where T : IFloatingPoint<T> => T.Atan(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Atan2<T>(T y, T x) where T : IFloatingPoint<T> => T.Atan2(y, x);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Atanh<T>(T value) where T : IFloatingPoint<T> => T.Atanh(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Sqrt<T>(T value) where T : IFloatingPoint<T> => T.Sqrt(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Cbrt<T>(T value) where T : IFloatingPoint<T> => T.Cbrt(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Ceiling<T>(T value) where T : IFloatingPoint<T> => T.Ceiling(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Floor<T>(T value) where T : IFloatingPoint<T> => T.Floor(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Truncate<T>(T value) where T : IFloatingPoint<T> => T.Truncate(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Exp<T>(T value) where T : IFloatingPoint<T> => T.Exp(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T IEEERemainder<T>(T dividend, T divisor) where T : IFloatingPoint<T> =>
		T.IEEERemainder(dividend, divisor);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TInteger ILogB<T, TInteger>(T value)
		where T : IFloatingPoint<T>
		where TInteger : IBinaryInteger<TInteger> =>
		T.ILogB<TInteger>(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsFinite<T>(T value) where T : IFloatingPoint<T> => T.IsFinite(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsInfinity<T>(T value) where T : IFloatingPoint<T> => T.IsInfinity(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNaN<T>(T value) where T : IFloatingPoint<T> => T.IsNaN(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNegative<T>(T value) where T : IFloatingPoint<T> => T.IsNegative(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNegativeInfinity<T>(T value) where T : IFloatingPoint<T> => T.IsNegativeInfinity(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPositiveInfinity<T>(T value) where T : IFloatingPoint<T> => T.IsPositiveInfinity(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNormal<T>(T value) where T : IFloatingPoint<T> => T.IsNormal(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsSubnormal<T>(T value) where T : IFloatingPoint<T> => T.IsSubnormal(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Log<T>(T value) where T : IFloatingPoint<T> => T.Log(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Log<T>(T value, T newBase) where T : IFloatingPoint<T> => T.Log(value, newBase);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Log10<T>(T value) where T : IFloatingPoint<T> => T.Log10(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Log2<T>(T value) where T : IFloatingPoint<T> => T.Log2(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Pow<T>(T value, T power) where T : IFloatingPoint<T> => T.Pow(value, power);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Round<T>(T value) where T : IFloatingPoint<T> => T.Round(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Round<T>(T value, MidpointRounding mode) where T : IFloatingPoint<T> => T.Round(value, mode);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Round<T, TInteger>(T value, TInteger digits)
		where T : IFloatingPoint<T>
		where TInteger : IBinaryInteger<TInteger> =>
		T.Round(value, digits);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Round<T, TInteger>(T value, TInteger digits, MidpointRounding mode)
		where T : IFloatingPoint<T>
		where TInteger : IBinaryInteger<TInteger> =>
		T.Round(value, digits, mode);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T ScaleB<T, TInteger>(T value, TInteger scale)
		where T : IFloatingPoint<T>
		where TInteger : IBinaryInteger<TInteger> =>
		T.ScaleB(value, scale);
#pragma warning restore CS1591
}
