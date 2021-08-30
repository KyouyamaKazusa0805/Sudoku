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
	/// <typeparam name="TNumber">The type of the number.</typeparam>
	/// <param name="value">The value.</param>
	/// <returns>The absolute result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TNumber Abs<TNumber>(TNumber value) where TNumber : INumber<TNumber> => TNumber.Abs(value);

	/// <summary>
	/// Produces the quotient and the remainder of two numbers.
	/// </summary>
	/// <typeparam name="TNumber">The type of the number.</typeparam>
	/// <param name="dividend">The dividend.</param>
	/// <param name="divisor">The divisor.</param>
	/// <returns>The quotient and the remainder of the specified numbers.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (TNumber Quotient, TNumber Remainder) DivRem<TNumber>(TNumber dividend, TNumber divisor)
		where TNumber : INumber<TNumber> =>
		TNumber.DivRem(dividend, divisor);

	/// <summary>
	/// Clamps the specified value. If the value is less than <paramref name="min"/>,
	/// the return value will be the same as <paramref name="min"/>; if the value is greater than
	/// <paramref name="max"/>, the return value will be the same as <paramref name="max"/>; else
	/// the <paramref name="value"/> itself. The details are displayed
	/// in the <see href="#returns">returns</see> part.
	/// </summary>
	/// <typeparam name="TNumber">The type of the number.</typeparam>
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
	public static TNumber Clamp<TNumber>(TNumber value, TNumber min, TNumber max)
		where TNumber : INumber<TNumber> =>
		TNumber.Clamp(value, min, max);

	/// <summary>
	/// Get the maximum value between <paramref name="val1"/> and <paramref name="val2"/>.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to check.</typeparam>
	/// <param name="val1">The first value to check.</param>
	/// <param name="val2">The second value to check.</param>
	/// <returns>The result value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TNumber Max<TNumber>(TNumber val1, TNumber val2) where TNumber : INumber<TNumber> =>
		TNumber.Max(val1, val2);

	/// <summary>
	/// Get the minimum value between <paramref name="val1"/> and <paramref name="val2"/>.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value to check.</typeparam>
	/// <param name="val1">The first value to check.</param>
	/// <param name="val2">The second value to check.</param>
	/// <returns>The result value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TNumber Min<TNumber>(TNumber val1, TNumber val2) where TNumber : INumber<TNumber> =>
		TNumber.Min(val1, val2);

	/// <summary>
	/// Get the sign of the value.
	/// </summary>
	/// <typeparam name="TNumber">The type of the value.</typeparam>
	/// <param name="value">The value.</param>
	/// <returns>The sign value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TNumber Sign<TNumber>(TNumber value) where TNumber : INumber<TNumber> => TNumber.Sign(value);

	/// <summary>
	/// Returns the cosine of the specified angle.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the angle. The angle must be a floating point.</typeparam>
	/// <param name="angle">The angle.</param>
	/// <returns>
	/// Returns the possible values as below:
	/// <list type="table">
	/// <item>
	/// <term>Normal cases</term>
	/// <description>The cosine of <paramref name="angle"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.NaN"/></term>
	/// <description>Returns <see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.NegativeInfinity"/></term>
	/// <description>Returns <see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.PositiveInfinity"/></term>
	/// <description>Returns <see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="IFloatingPoint{TSelf}.NegativeInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.PositiveInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.NaN"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Cos<TFloatingPoint>(TFloatingPoint angle)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Cos(angle);

	/// <summary>
	/// Returns the hyperbolic cosine of the specified angle.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the angle. The angle must be a floating point.</typeparam>
	/// <param name="angle">The angle.</param>
	/// <returns>
	/// Returns the possible values as below:
	/// <list type="table">
	/// <item>
	/// <term>Normal cases</term>
	/// <description>The hyperbolic cosine of <paramref name="angle"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.NaN"/></term>
	/// <description>Returns <see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.NegativeInfinity"/></term>
	/// <description>Returns <see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.PositiveInfinity"/></term>
	/// <description>Returns <see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="IFloatingPoint{TSelf}.NegativeInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.PositiveInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.NaN"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Cosh<TFloatingPoint>(TFloatingPoint angle)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Cosh(angle);

	/// <summary>
	/// Returns the sine of the specified angle.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the angle. The angle must be a floating point.</typeparam>
	/// <param name="angle">The angle.</param>
	/// <returns>
	/// Returns the possible values as below:
	/// <list type="table">
	/// <item>
	/// <term>Normal cases</term>
	/// <description>The sine of <paramref name="angle"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.NaN"/></term>
	/// <description>Returns <see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.NegativeInfinity"/></term>
	/// <description>Returns <see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.PositiveInfinity"/></term>
	/// <description>Returns <see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="IFloatingPoint{TSelf}.NegativeInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.PositiveInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.NaN"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Sin<TFloatingPoint>(TFloatingPoint angle)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Sin(angle);

	/// <summary>
	/// Returns the hyperbolic sine of the specified angle.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the angle. The angle must be a floating point.</typeparam>
	/// <param name="angle">The angle.</param>
	/// <returns>
	/// Returns the possible values as below:
	/// <list type="table">
	/// <item>
	/// <term>Normal cases</term>
	/// <description>The hyperbolic sine of <paramref name="angle"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.NaN"/></term>
	/// <description>Returns <see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.NegativeInfinity"/></term>
	/// <description>Returns <see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.PositiveInfinity"/></term>
	/// <description>Returns <see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="IFloatingPoint{TSelf}.NegativeInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.PositiveInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.NaN"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Sinh<TFloatingPoint>(TFloatingPoint angle)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Sinh(angle);

	/// <summary>
	/// Returns the tangent of the specified angle.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the angle. The angle must be a floating point.</typeparam>
	/// <param name="angle">The angle.</param>
	/// <returns>
	/// Returns the possible values as below:
	/// <list type="table">
	/// <item>
	/// <term>Normal cases</term>
	/// <description>The tangent of <paramref name="angle"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.NaN"/></term>
	/// <description>Returns <see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.NegativeInfinity"/></term>
	/// <description>Returns <see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.PositiveInfinity"/></term>
	/// <description>Returns <see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="IFloatingPoint{TSelf}.NegativeInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.PositiveInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.NaN"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Tan<TFloatingPoint>(TFloatingPoint angle)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Tan(angle);

	/// <summary>
	/// Returns the hyperbolic tangent of the specified angle.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the angle. The angle must be a floating point.</typeparam>
	/// <param name="angle">The angle.</param>
	/// <returns>
	/// Returns the possible values as below:
	/// <list type="table">
	/// <item>
	/// <term>Normal cases</term>
	/// <description>The hyperbolic tangent of <paramref name="angle"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.NaN"/></term>
	/// <description>Returns <see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.NegativeInfinity"/></term>
	/// <description>Returns <see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.PositiveInfinity"/></term>
	/// <description>Returns <see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="IFloatingPoint{TSelf}.NegativeInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.PositiveInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.NaN"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Tanh<TFloatingPoint>(TFloatingPoint angle)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Tanh(angle);

	/// <summary>
	/// Returns the angle whose cosine is the specified value.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the angle. The angle must be a floating point.</typeparam>
	/// <param name="angle">The angle.</param>
	/// <returns>
	/// The angle, theta, measured in radians, such that:
	/// <list type="table">
	/// <item>
	/// <term><![CDATA[0 <= theta <= pi]]></term>
	/// <description>Normal case</description>
	/// </item>
	/// <item>
	/// <term><paramref name="angle"/> <![CDATA[< 1]]> or <paramref name="angle"/> <![CDATA[> 1]]></term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/></description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.NaN"/></term>
	/// <description><paramref name="angle"/></description>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="IFloatingPoint{TSelf}.NaN"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Acos<TFloatingPoint>(TFloatingPoint angle)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Acos(angle);

	/// <summary>
	/// Returns the angle whose hyperbolic cosine is the specified value.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the angle. The angle must be a floating point.</typeparam>
	/// <param name="angle">The angle.</param>
	/// <returns>
	/// The angle, theta, measured in radians, such that:
	/// <list type="table">
	/// <item>
	/// <term><![CDATA[0 <= theta <= pi]]></term>
	/// <description>Normal case</description>
	/// </item>
	/// <item>
	/// <term><paramref name="angle"/> <![CDATA[< 1]]> or <paramref name="angle"/> <![CDATA[> 1]]></term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/></description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.NaN"/></term>
	/// <description><paramref name="angle"/></description>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="IFloatingPoint{TSelf}.NaN"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Acosh<TFloatingPoint>(TFloatingPoint angle)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Acosh(angle);

	/// <summary>
	/// Returns the angle whose sine is the specified value.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the angle. The angle must be a floating point.</typeparam>
	/// <param name="angle">The angle.</param>
	/// <returns>
	/// The angle, theta, measured in radians, such that:
	/// <list type="table">
	/// <item>
	/// <term><![CDATA[0 <= theta <= pi]]></term>
	/// <description>Normal case</description>
	/// </item>
	/// <item>
	/// <term><paramref name="angle"/> <![CDATA[< 1]]> or <paramref name="angle"/> <![CDATA[> 1]]></term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/></description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.NaN"/></term>
	/// <description><paramref name="angle"/></description>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="IFloatingPoint{TSelf}.NaN"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Asin<TFloatingPoint>(TFloatingPoint angle)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Asin(angle);

	/// <summary>
	/// Returns the angle whose hyperbolic sine is the specified value.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the angle. The angle must be a floating point.</typeparam>
	/// <param name="angle">The angle.</param>
	/// <returns>
	/// The angle, theta, measured in radians, such that:
	/// <list type="table">
	/// <item>
	/// <term><![CDATA[0 <= theta <= pi]]></term>
	/// <description>Normal case</description>
	/// </item>
	/// <item>
	/// <term><paramref name="angle"/> <![CDATA[< 1]]> or <paramref name="angle"/> <![CDATA[> 1]]></term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/></description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.NaN"/></term>
	/// <description><paramref name="angle"/></description>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="IFloatingPoint{TSelf}.NaN"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Asinh<TFloatingPoint>(TFloatingPoint angle)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> => TFloatingPoint.Asinh(angle);

	/// <summary>
	/// Returns the angle whose tangent is the specified value.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the angle. The angle must be a floating point.</typeparam>
	/// <param name="angle">The angle.</param>
	/// <returns>
	/// The angle, theta, measured in radians, such that:
	/// <list type="table">
	/// <item>
	/// <term><![CDATA[0 <= theta <= pi]]></term>
	/// <description>Normal case</description>
	/// </item>
	/// <item>
	/// <term><paramref name="angle"/> <![CDATA[< 1]]> or <paramref name="angle"/> <![CDATA[> 1]]></term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/></description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.NaN"/></term>
	/// <description><paramref name="angle"/></description>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="IFloatingPoint{TSelf}.NaN"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Atan<TFloatingPoint>(TFloatingPoint angle)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> => TFloatingPoint.Atan(angle);

	/// <summary>
	/// Returns the angle whose tangent is the quotient of two specified numbers.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the number.</typeparam>
	/// <param name="y">The <c>y</c> coordinate of a point.</param>
	/// <param name="x">The <c>x</c> coordinate of a point.</param>
	/// <returns>
	/// <para>
	/// An angle, <c>theta</c>, measured in radians, such that <c><![CDATA[-pi <= theta <= pi]]></c>,
	/// and <c>tan(<c>theta</c>) = <paramref name="y"/> / <paramref name="x"/></c>,
	/// where <c>(<paramref name="x"/>, <paramref name="y"/>)</c> is a point in the Cartesian plane.
	/// </para>
	/// <para>
	/// Observe the following:
	/// <list type="bullet">
	/// <item>For (<paramref name="x"/>, <paramref name="y"/>) in quadrant 1, <![CDATA[0 < theta < pi/2]]>.</item>
	/// <item>For (<paramref name="x"/>, <paramref name="y"/>) in quadrant 2, <![CDATA[pi/2 < theta <= pi]]>.</item>
	/// <item>For (<paramref name="x"/>, <paramref name="y"/>) in quadrant 3, <![CDATA[-pi < theta < -pi/2]]>.</item>
	/// <item>For (<paramref name="x"/>, <paramref name="y"/>) in quadrant 4, <![CDATA[-pi/2 < theta < 0]]>.</item>
	/// </list>
	/// </para>
	/// <para>
	/// For points on the boundaries of the quadrants, the return value is the following:
	/// <list type="bullet">
	/// <item>If <paramref name="y"/> is <c>0</c> and <paramref name="x"/> isn't negative, <c>theta</c> = 0.</item>
	/// <item>If <paramref name="y"/> is <c>0</c> and <paramref name="x"/> is negative, <c>theta</c> = pi.</item>
	/// <item>If <paramref name="y"/> is positive and <paramref name="x"/> is 0, <c>theta</c> = pi/2.</item>
	/// <item>If <paramref name="y"/> is negative and <paramref name="x"/> is 0, <c>theta</c> = -pi/2.</item>
	/// <item>If <paramref name="y"/> is <c>0</c> and <paramref name="y"/> is <c>0</c>, <c>theta</c> = 0.</item>
	/// <item>
	/// If <paramref name="x"/> or <paramref name="y"/> is <see cref="IFloatingPoint{TSelf}.NaN"/>,
	/// or if <paramref name="x"/> and <paramref name="y"/> are
	/// either <see cref="IFloatingPoint{TSelf}.PositiveInfinity"/> or
	/// <see cref="IFloatingPoint{TSelf}.NegativeInfinity"/>, <see cref="IFloatingPoint{TSelf}.NaN"/>.
	/// </item>
	/// </list>
	/// </para>
	/// </returns>
	/// <seealso cref="IFloatingPoint{TSelf}.NegativeInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.PositiveInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.NaN"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Atan2<TFloatingPoint>(TFloatingPoint y, TFloatingPoint x)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Atan2(y, x);

	/// <summary>
	/// Returns the angle whose hyperbolic tangent is the specified value.
	/// </summary>
	/// <typeparam name="TFloatingPoint"></typeparam>
	/// <param name="value"></param>
	/// <returns>
	/// The angle, theta, measured in radians, such that:
	/// <list type="table">
	/// <item>
	/// <term><![CDATA[0 <= theta <= pi]]></term>
	/// <description>Normal case</description>
	/// </item>
	/// <item>
	/// <term><paramref name="value"/> <![CDATA[< 1]]> or <paramref name="value"/> <![CDATA[> 1]]></term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/></description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.NaN"/></term>
	/// <description><paramref name="value"/></description>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="IFloatingPoint{TSelf}.NaN"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Atanh<TFloatingPoint>(TFloatingPoint value)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Atanh(value);

	/// <summary>
	/// Returns the square root of a specified number.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the number.</typeparam>
	/// <param name="value">The number.</param>
	/// <returns>
	/// One of the values in the following table.
	/// <list type="table">
	/// <item>
	/// <term>Non-negative number (i.e. <![CDATA[>= 0]]>)</term>
	/// <description>The positive square root of <paramref name="value"/>.</description>
	/// </item>
	/// <item>
	/// <term>Negative</term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.NaN"/></term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.PositiveInfinity"/></term>
	/// <description><see cref="IFloatingPoint{TSelf}.PositiveInfinity"/>.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="IFloatingPoint{TSelf}.NegativeInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.PositiveInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.NaN"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Sqrt<TFloatingPoint>(TFloatingPoint value)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Sqrt(value);

	/// <summary>
	/// Returns the cube root of a specified number.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the number.</typeparam>
	/// <param name="value">The number.</param>
	/// <returns>
	/// One of the values in the following table.
	/// <list type="table">
	/// <item>
	/// <term>Non-negative number (i.e. <![CDATA[>= 0]]>)</term>
	/// <description>The positive cube root of <paramref name="value"/>.</description>
	/// </item>
	/// <item>
	/// <term>Negative</term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.NaN"/></term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equals to <see cref="IFloatingPoint{TSelf}.PositiveInfinity"/></term>
	/// <description><see cref="IFloatingPoint{TSelf}.PositiveInfinity"/>.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="IFloatingPoint{TSelf}.NegativeInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.PositiveInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.NaN"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Cbrt<TFloatingPoint>(TFloatingPoint value)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Cbrt(value);

	/// <summary>
	/// Returns the smallest integral value that is greater than or equal to the specified number.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the number.</typeparam>
	/// <param name="value">The number.</param>
	/// <returns>
	/// The smallest integral value that is greater than or equal to <paramref name="value"/>.
	/// Note that this method returns a <typeparamref name="TFloatingPoint"/> instead of an integral type.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Ceiling<TFloatingPoint>(TFloatingPoint value)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Ceiling(value);

	/// <summary>
	/// Returns the biggest integral value that is less than or equal to the specified number.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the number.</typeparam>
	/// <param name="value">The number.</param>
	/// <returns>
	/// The biggest integral value that is less than or equal to <paramref name="value"/>.
	/// Note that this method returns a <typeparamref name="TFloatingPoint"/> instead of an integral type.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Floor<TFloatingPoint>(TFloatingPoint value)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Floor(value);

	/// <summary>
	/// Calculates the integral part of a specified number.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the number.</typeparam>
	/// <param name="value">The number.</param>
	/// <returns>
	/// The integral part of <paramref name="value"/>; that is, the number that remains
	/// after any fractional digits have been discarded.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Truncate<TFloatingPoint>(TFloatingPoint value)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Truncate(value);

	/// <summary>
	/// Returns <c>e</c> raised to the specified power.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the number.</typeparam>
	/// <param name="value">A number specifying a power.</param>
	/// <returns>
	/// The number <c>e</c> raised to the power <paramref name="value"/>. Returns:
	/// <list type="table">
	/// <item>
	/// <term>
	/// <paramref name="value"/> equals <see cref="IFloatingPoint{TSelf}.NaN"/> or
	/// <see cref="IFloatingPoint{TSelf}.PositiveInfinity"/>
	/// </term>
	/// <description>That value is returned.</description>
	/// </item>
	/// <item>
	/// <term><paramref name="value"/> equals <see cref="IFloatingPoint{TSelf}.NegativeInfinity"/></term>
	/// <description>0 is returned.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="IFloatingPoint{TSelf}.NegativeInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.PositiveInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.NaN"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Exp<TFloatingPoint>(TFloatingPoint value)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Exp(value);

	/// <summary>
	/// Returns the remainder resulting from the division of a specified number by another specified number.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the dividend and the divisor.</typeparam>
	/// <param name="dividend">The dividend.</param>
	/// <param name="divisor">The divisor.</param>
	/// <returns>
	/// <para>
	/// A number equal to <c><paramref name="dividend"/> - <paramref name="divisor"/> * Q</c>,
	/// where <c>Q</c> is the quotient of <c><paramref name="dividend"/> / <paramref name="divisor"/></c>
	/// rounded to the nearest integer (if <c><paramref name="dividend"/> / <paramref name="divisor"/></c>
	/// falls halfway between two integers, the even integer is returned).
	/// </para>
	/// <para>
	/// Returns:
	/// <list type="table">
	/// <item>
	/// <term>If <c><paramref name="dividend"/> - <paramref name="divisor"/> * Q</c> is 0</term>
	/// <description>
	/// The value <c>+0</c> is returned if f <paramref name="dividend"/> is positive
	/// or <c>-0</c> if <paramref name="dividend"/> is negative.
	/// </description>
	/// </item>
	/// <item>
	/// <term>If <paramref name="divisor"/> = 0</term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/> is returned.</description>
	/// </item>
	/// </list>
	/// </para>
	/// </returns>
	/// <seealso cref="IFloatingPoint{TSelf}.NaN"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint IEEERemainder<TFloatingPoint>(TFloatingPoint dividend, TFloatingPoint divisor)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.IEEERemainder(dividend, divisor);

	/// <summary>
	/// Returns the base 2 integer logarithm of a specified number.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the number.</typeparam>
	/// <typeparam name="TInteger">The type of the return value.</typeparam>
	/// <param name="value">The number whose logarithm is to be found.</param>
	/// <returns>
	/// One of the values in the following table:
	/// <list type="table">
	/// <listheader>
	/// <term><paramref name="value"/> parameter</term>
	/// <description>Return value</description>
	/// </listheader>
	/// <item>
	/// <term>Default</term>
	/// <description>
	/// The base 2 integer log of <paramref name="value"/>, i.e. <c>(int)log2(<paramref name="value"/>)</c>.
	/// </description>
	/// </item>
	/// <item>
	/// <term>Zero</term>
	/// <description>-<see cref="IMinMaxValue{TSelf}.MinValue"/>.</description>
	/// </item>
	/// <item>
	/// <term>
	/// Equal to <see cref="IFloatingPoint{TSelf}.NaN"/>, <see cref="IFloatingPoint{TSelf}.PositiveInfinity"/>
	/// or <see cref="IFloatingPoint{TSelf}.NegativeInfinity"/>
	/// </term>
	/// <description>-<see cref="IMinMaxValue{TSelf}.MaxValue"/>.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="IFloatingPoint{TSelf}.NaN"/>
	/// <seealso cref="IFloatingPoint{TSelf}.PositiveInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.NegativeInfinity"/>
	/// <seealso cref="IMinMaxValue{TSelf}.MinValue"/>
	/// <seealso cref="IMinMaxValue{TSelf}.MaxValue"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TInteger ILogB<TFloatingPoint, TInteger>(TFloatingPoint value)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint>
		where TInteger : IBinaryInteger<TInteger> =>
		TFloatingPoint.ILogB<TInteger>(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsFinite<T>(T value) where T : IFloatingPoint<T> => T.IsFinite(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsInfinity<TFloatingPoint>(TFloatingPoint value)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> => TFloatingPoint.IsInfinity(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNaN<TFloatingPoint>(TFloatingPoint value)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.IsNaN(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNegative<TFloatingPoint>(TFloatingPoint value)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.IsNegative(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNegativeInfinity<TFloatingPoint>(TFloatingPoint value)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.IsNegativeInfinity(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPositiveInfinity<TFloatingPoint>(TFloatingPoint value)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.IsPositiveInfinity(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNormal<TFloatingPoint>(TFloatingPoint value)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.IsNormal(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsSubnormal<TFloatingPoint>(TFloatingPoint value)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.IsSubnormal(value);

	/// <summary>
	/// Returns the natural (base <c>e</c>) logarithm of a specified number.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the number.</typeparam>
	/// <param name="value">The number whose logarithm is to be found.</param>
	/// <returns>
	/// One of the values in the following table:
	/// <list type="table">
	/// <listheader>
	/// <term><paramref name="value"/> parameter</term>
	/// <description>Return value</description>
	/// </listheader>
	/// <item>
	/// <term>Positive</term>
	/// <description>
	/// The natural logarithm of <paramref name="value"/>; that is,
	/// <c>ln d</c>, or <c>log e <paramref name="value"/></c>.
	/// </description>
	/// </item>
	/// <item>
	/// <term>Zero</term>
	/// <description><see cref="IFloatingPoint{TSelf}.NegativeInfinity"/></description>
	/// </item>
	/// <item>
	/// <term>Negative</term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/></description>
	/// </item>
	/// <item>
	/// <term>Equal to <see cref="IFloatingPoint{TSelf}.NaN"/></term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/>.</description>
	/// </item>
	/// <item>
	/// <term>Equal to <see cref="IFloatingPoint{TSelf}.PositiveInfinity"/>.</term>
	/// <description><see cref="IFloatingPoint{TSelf}.PositiveInfinity"/></description>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="IFloatingPoint{TSelf}.PositiveInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.NegativeInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.NaN"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Log<TFloatingPoint>(TFloatingPoint value)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Log(value);

	/// <summary>
	/// Returns the logarithm of a specified number in a specified base.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the number.</typeparam>
	/// <param name="value">The number whose logarithm is to be found.</param>
	/// <param name="newBase">The base of the logarithm.</param>
	/// <returns>
	/// One of the values in the following table:
	/// <list type="table">
	/// <listheader>
	/// <term><paramref name="value"/> and <paramref name="newBase"/></term>
	/// <description>Return value</description>
	/// </listheader>
	/// <item>
	/// <term>
	/// <paramref name="value"/> <![CDATA[> 0]]>, <![CDATA[0 <]]> <paramref name="newBase"/> <![CDATA[< 1]]>
	/// or <paramref name="newBase"/> <![CDATA[> 1]]>
	/// </term>
	/// <description>log <paramref name="newBase"/> (<paramref name="value"/>)</description>
	/// </item>
	/// <item>
	/// <term><paramref name="value"/> <![CDATA[< 0]]>, any value</term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/></description>
	/// </item>
	/// <item>
	/// <term>Any value, <paramref name="newBase"/> <![CDATA[< 0]]></term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/></description>
	/// </item>
	/// <item>
	/// <term><paramref name="value"/> != 1, <paramref name="newBase"/> = 0</term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/></description>
	/// </item>
	/// <item>
	/// <term>
	/// <paramref name="value"/> != 1,
	/// <paramref name="newBase"/> = <see cref="IFloatingPoint{TSelf}.PositiveInfinity"/>
	/// </term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/></description>
	/// </item>
	/// <item>
	/// <term><paramref name="value"/> = <see cref="IFloatingPoint{TSelf}.NaN"/>, any value</term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/></description>
	/// </item>
	/// <item>
	/// <term>Any value, <paramref name="newBase"/> = <see cref="IFloatingPoint{TSelf}.NaN"/></term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/></description>
	/// </item>
	/// <item>
	/// <term>Any value, <paramref name="newBase"/> = 1</term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/></description>
	/// </item>
	/// <item>
	/// <term><paramref name="value"/> = 0, <![CDATA[0 <]]> <paramref name="newBase"/> <![CDATA[< 1]]>
	/// </term>
	/// <description><see cref="IFloatingPoint{TSelf}.PositiveInfinity"/></description>
	/// </item>
	/// <item>
	/// <term><paramref name="value"/> = 0, <paramref name="newBase"/> <![CDATA[> 1]]></term>
	/// <description><see cref="IFloatingPoint{TSelf}.NegativeInfinity"/></description>
	/// </item>
	/// <item>
	/// <term>
	/// <paramref name="value"/> = <see cref="IFloatingPoint{TSelf}.PositiveInfinity"/>,
	/// <![CDATA[0 <]]> <paramref name="newBase"/> <![CDATA[< 1]]>
	/// </term>
	/// <description><see cref="IFloatingPoint{TSelf}.NegativeInfinity"/></description>
	/// </item>
	/// <item>
	/// <term>
	/// <paramref name="value"/> = <see cref="IFloatingPoint{TSelf}.PositiveInfinity"/>
	/// <paramref name="newBase"/> <![CDATA[> 1]]>
	/// </term>
	/// <description><see cref="IFloatingPoint{TSelf}.PositiveInfinity"/></description>
	/// </item>
	/// <item>
	/// <term><paramref name="value"/> = 1, <paramref name="newBase"/> = 0</term>
	/// <description>0</description>
	/// </item>
	/// <item>
	/// <term>
	/// <paramref name="value"/> = 1,
	/// <paramref name="newBase"/> = <see cref="IFloatingPoint{TSelf}.PositiveInfinity"/>
	/// </term>
	/// <description>0</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="IFloatingPoint{TSelf}.PositiveInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.NegativeInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.NaN"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Log<TFloatingPoint>(TFloatingPoint value, TFloatingPoint newBase)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Log(value, newBase);

	/// <summary>
	/// Returns the base 10 logarithm of a specified number.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the number.</typeparam>
	/// <param name="value">The number.</param>
	/// <returns>
	/// One of the values in the following table:
	/// <list type="table">
	/// <listheader>
	/// <term><paramref name="value"/> parameter</term>
	/// <description>Return value</description>
	/// </listheader>
	/// <item>
	/// <term>Positive</term>
	/// <description>The base 10 log of d; that is, <c>log 10 <paramref name="value"/></c>.</description>
	/// </item>
	/// <item>
	/// <term>Zero</term>
	/// <description><see cref="IFloatingPoint{TSelf}.NegativeInfinity"/></description>
	/// </item>
	/// <item>
	/// <term>Negative</term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/></description>
	/// </item>
	/// <item>
	/// <term>Equal to <see cref="IFloatingPoint{TSelf}.NaN"/></term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/></description>
	/// </item>
	/// <item>
	/// <term>Equal to <see cref="IFloatingPoint{TSelf}.PositiveInfinity"/></term>
	/// <description><see cref="IFloatingPoint{TSelf}.PositiveInfinity"/></description>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="IFloatingPoint{TSelf}.PositiveInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.NegativeInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.NaN"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Log10<TFloatingPoint>(TFloatingPoint value)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Log10(value);

	/// <summary>
	/// Returns the base 2 logarithm of a specified number.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the number.</typeparam>
	/// <param name="value">The number.</param>
	/// <returns>
	/// One of the values in the following table:
	/// <list type="table">
	/// <listheader>
	/// <term><paramref name="value"/> parameter</term>
	/// <description>Return value</description>
	/// </listheader>
	/// <item>
	/// <term>Positive</term>
	/// <description>The base 2 log of d; that is, <c>log 2 <paramref name="value"/></c>.</description>
	/// </item>
	/// <item>
	/// <term>Zero</term>
	/// <description><see cref="IFloatingPoint{TSelf}.NegativeInfinity"/></description>
	/// </item>
	/// <item>
	/// <term>Negative</term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/></description>
	/// </item>
	/// <item>
	/// <term>Equal to <see cref="IFloatingPoint{TSelf}.NaN"/></term>
	/// <description><see cref="IFloatingPoint{TSelf}.NaN"/></description>
	/// </item>
	/// <item>
	/// <term>Equal to <see cref="IFloatingPoint{TSelf}.PositiveInfinity"/></term>
	/// <description><see cref="IFloatingPoint{TSelf}.PositiveInfinity"/></description>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="IFloatingPoint{TSelf}.PositiveInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.NegativeInfinity"/>
	/// <seealso cref="IFloatingPoint{TSelf}.NaN"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Log2<TFloatingPoint>(TFloatingPoint value)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Log2(value);

	/// <summary>
	/// Returns a specified number raised to the specified power.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the number.</typeparam>
	/// <param name="value">A double-precision floating-point number to be raised to a power.</param>
	/// <param name="power">A double-precision floating-point number that specifies a power.</param>
	/// <returns>The number <paramref name="value"/> raised to the power <paramref name="power"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Pow<TFloatingPoint>(TFloatingPoint value, TFloatingPoint power)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Pow(value, power);

	/// <summary>
	/// Rounds a double-precision floating-point value to the nearest integral value,
	/// and rounds midpoint values to the nearest even number.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the number.</typeparam>
	/// <param name="value">A double-precision floating-point number to be rounded.</param>
	/// <returns>
	/// <para>
	/// The integer nearest <paramref name="value"/>.
	/// If the fractional component of <paramref name="value"/> is halfway between two integers,
	/// one of which is even and the other odd, then the even number is returned.
	/// </para>
	/// <para>
	/// Note that this method returns a <typeparamref name="TFloatingPoint"/> instead of an integral type.
	/// </para>
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Round<TFloatingPoint>(TFloatingPoint value)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Round(value);

	/// <summary>
	/// Rounds a double-precision floating-point value to the nearest integer, and uses
	/// the specified rounding convention for midpoint values.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the number.</typeparam>
	/// <param name="value">A double-precision floating-point number to be rounded.</param>
	/// <param name="mode">Specification for how to round value if it is midway between two other numbers.</param>
	/// <returns>
	/// <para>
	/// The integer nearest value. If value is halfway between two integers, one of which
	/// is even and the other odd, then mode determines which of the two is returned.
	/// </para>
	/// <para>
	/// Note that this method returns a <typeparamref name="TFloatingPoint"/> instead of an integral type.
	/// </para>
	/// </returns>
	/// <exception cref="ArgumentException">
	/// <paramref name="mode"/> is not a valid value of <see cref="MidpointRounding"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Round<TFloatingPoint>(TFloatingPoint value, MidpointRounding mode)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint> =>
		TFloatingPoint.Round(value, mode);

	/// <summary>
	/// Rounds a double-precision floating-point value to a specified number of fractional digits,
	/// and rounds midpoint values to the nearest even number.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the <paramref name="value"/>.</typeparam>
	/// <typeparam name="TInteger">The type of <paramref name="digits"/>.</typeparam>
	/// <param name="value">A double-precision floating-point number to be rounded.</param>
	/// <param name="digits">The number of fractional digits in the return value.</param>
	/// <returns>
	/// The number nearest to value that contains a number of fractional digits equal to digits.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="digits"/> is less than 0 or greater than 15.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Round<TFloatingPoint, TInteger>(TFloatingPoint value, TInteger digits)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint>
		where TInteger : IBinaryInteger<TInteger> =>
		TFloatingPoint.Round(value, digits);

	/// <summary>
	/// Rounds a double-precision floating-point value to a specified number of fractional digits,
	/// and uses the specified rounding convention for midpoint values.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the <paramref name="value"/>.</typeparam>
	/// <typeparam name="TInteger">The type of <paramref name="digits"/>.</typeparam>
	/// <param name="value">A double-precision floating-point number to be rounded.</param>
	/// <param name="digits">The number of fractional digits in the return value.</param>
	/// <param name="mode">Specification for how to round value if it is midway between two other numbers.</param>
	/// <returns>
	/// The number nearest to value that has a number of fractional digits equal to digits.
	/// If value has fewer fractional digits than digits, value is returned unchanged.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <paramref name="digits"/> is less than 0 or greater than 15.
	/// </exception>
	/// <exception cref="ArgumentException">
	/// <paramref name="mode"/> is not a valid value of <see cref="MidpointRounding"/>.
	/// </exception>
	/// <seealso cref="MidpointRounding"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint Round<TFloatingPoint, TInteger>(TFloatingPoint value, TInteger digits, MidpointRounding mode)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint>
		where TInteger : IBinaryInteger<TInteger> =>
		TFloatingPoint.Round(value, digits, mode);

	/// <summary>
	/// Returns <c><paramref name="value"/> * 2 ^ <paramref name="scale"/></c> computed efficiently.
	/// </summary>
	/// <typeparam name="TFloatingPoint">The type of the number.</typeparam>
	/// <typeparam name="TInteger">The type of the return value.</typeparam>
	/// <param name="value">A double-precision floating-point number that specifies the base value.</param>
	/// <param name="scale">An integer that specifies the power.</param>
	/// <returns><c><paramref name="value"/> * 2 ^ <paramref name="scale"/></c> computed efficiently.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFloatingPoint ScaleB<TFloatingPoint, TInteger>(TFloatingPoint value, TInteger scale)
		where TFloatingPoint : IFloatingPoint<TFloatingPoint>
		where TInteger : IBinaryInteger<TInteger> =>
		TFloatingPoint.ScaleB(value, scale);
}
