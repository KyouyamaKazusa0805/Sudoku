namespace System;

/// <summary>
/// Defines a type that only contains 16 bits to represent a floating-point number,
/// known as <c>float16_t</c> in C++.
/// </summary>
/// <remarks>
/// <para>The type is re-implemented from .NET library of type <see cref="Half"/>.</para>
/// <para>
/// In addition, this type is based on this repository (<see href="https://github.com/artyom-beilis/float16">artyom-beilis/float16</see>).
/// If you want to learn more about the type, please visit that link for more information.
/// </para>
/// </remarks>
/// <seealso cref="Half"/>
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
public readonly struct float16_t : IMinMaxValue<float16_t>, IEquatable<float16_t>, IComparable<float16_t>
{
	/// <summary>
	/// Indicates the sign-part mask.
	/// </summary>
	private const ushort SignMask = 0x8000;

	/// <summary>
	/// Indicates the exponent-part mask.
	/// </summary>
	private const short ExponentMask = 0x7C00;

	/// <summary>
	/// Indicates the raw value of the constant <c>NaN</c>.
	/// </summary>
	private const short NaNRawValue = 0x7FFF;


	/// <inheritdoc cref="IMinMaxValue{TSelf}.MinValue"/>
	public static readonly float16_t MinValue = new(0b0_00001_0000000000);

	/// <inheritdoc cref="IMinMaxValue{TSelf}.MaxValue"/>
	public static readonly float16_t MaxValue = new(0b0_11110_1111111111);


	/// <summary>
	/// Indicates the inner mask bits.
	/// </summary>
	private readonly short _mask;


	/// <summary>
	/// Initializes a <see cref="float16_t"/> instance via the specified mask of 16-bit length.
	/// </summary>
	/// <param name="mask">The mask of 16-bit length.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float16_t(short mask) => _mask = mask;


	/// <summary>
	/// Determines whether the specified value is positive or negative infinity.
	/// </summary>
	public bool IsInfinity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_mask & NaNRawValue) == ExponentMask;
	}

	/// <inheritdoc/>
	static float16_t IMinMaxValue<float16_t>.MinValue => MinValue;

	/// <inheritdoc/>
	static float16_t IMinMaxValue<float16_t>.MaxValue => MaxValue;


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] object? obj) =>
		obj is float16_t comparer && this == comparer;

	/// <inheritdoc/>
	public bool Equals(float16_t other) => this == other;

	/// <inheritdoc/>
	public int CompareTo(float16_t other) =>
		f16_gt(_mask, other._mask) ? 1 : f16_eq(_mask, other._mask) ? 0 : -1;

	/// <inheritdoc/>
	public override int GetHashCode() => _mask;

	/// <inheritdoc/>
	public override string ToString() => throw new NotImplementedException("Will be impl'ed later.");


	/// <summary>
	/// Determines whether the specified value is <c>0</c>.
	/// </summary>
	/// <param name="_mask">The mask.</param>
	/// <returns>The <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool GetIsZero(short _mask) => (_mask & NaNRawValue) == 0;

	/// <summary>
	/// Determines whether the specified value is invalid.
	/// </summary>
	/// <param name="_mask">The mask.</param>
	/// <returns>The <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool GetIsInvalid(short _mask) => (_mask & ExponentMask) == ExponentMask;

	/// <summary>
	/// Determines whether the specified value isn't a number.
	/// </summary>
	/// <param name="_mask">The mask.</param>
	/// <returns>The <see cref="bool"/> result.</returns>
	private static bool GetIsNaN(short _mask) => (_mask & NaNRawValue) > ExponentMask;

	/// <summary>
	/// Gets the mantissa value.
	/// </summary>
	/// <param name="_mask">The mask.</param>
	/// <returns>The mantissa value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static short GetMantissa(short _mask) =>
		(short)((_mask & 1023 | _mask & ExponentMask) == 0 ? 0 : 1024);

	/// <summary>
	/// Indicates the exponent value.
	/// </summary>
	/// <param name="_mask">The mask.</param>
	/// <returns>The exponent value.</returns>
	private static short GetExponent(short _mask) => (short)((_mask & ExponentMask) >> 10);

	/// <summary>
	/// Indicates the signed infinity value.
	/// </summary>
	/// <param name="_mask">The mask.</param>
	/// <returns>The signed infinity value.</returns>
	private static short GetSignedInfinity(short _mask) => (short)(_mask & SignMask | ExponentMask);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static short f16_add(short a, short b)
	{
		if (((a ^ b) & 0x8000) != 0)
		{
			return f16_sub(a, (short)(b ^ 0x8000));
		}

		short sign = (short)(a & 0x8000);
		a &= 0x7FFF;
		b &= 0x7FFF;
		if (a < b)
		{
			short x = a;
			a = b;
			b = x;
		}

		if (a >= 0x7C00 || b >= 0x7C00)
		{
			if (a > 0x7C00 || b > 0x7C00)
			{
				return 0x7FFF;
			}

			return (short)(0x7C00 | sign);
		}

		short ax = (short)(a & 0x7C00), bx = (short)(b & 0x7C00);
		short exp_diff = (short)(ax - bx), exp_part = ax;
		if (exp_diff != 0)
		{
			int shift = exp_diff >> 10;
			if (bx != 0)
			{
				b = (short)(((b & 1023) | 1024) >> shift);
			}
			else
			{
				b >>= shift - 1;
			}
		}
		else
		{
			if (bx == 0)
			{
				return (short)((short)(a + b) | sign);
			}
			else
			{
				b = (short)((b & 1023) | 1024);
			}
		}

		short r = (short)(a + b);
		if ((r & 0x7C00) != exp_part)
		{
			ushort am = (ushort)((a & 1023) | 1024);
			ushort new_m = (ushort)((am + b) >> 1);
			r = (short)((exp_part + 0x400) | (1023 & new_m));
		}
		if ((ushort)r >= 0x7C00u)
		{
			return (short)(sign | 0x7C00);
		}

		return (short)(r | sign);
	}

	private static short f16_sub(short ain, short bin)
	{
		ushort a = (ushort)ain, b = (ushort)bin;
		if (((a ^ b) & 0x8000) != 0)
		{
			return f16_add((short)a, (short)(b ^ 0x8000));
		}

		ushort sign = (ushort)(a & 0x8000);
		a <<= 1;
		b <<= 1;
		if (a < b)
		{
			ushort x = a;
			a = b;
			b = x;
			sign ^= 0x8000;
		}

		ushort ax = (ushort)(a & 0xF800), bx = (ushort)(b & 0xF800);
		if (a >= 0xf800 || b >= 0xf800)
		{
			if (a > 0xF800 || b > 0xF800 || a == b)
			{
				return 0x7FFF;
			}

			ushort res = (ushort)(sign | 0x7C00);
			return (short)(a == 0xf800 ? res : (res ^ 0x8000));
		}

		int exp_diff = ax - bx;
		ushort exp_part = ax;
		if (exp_diff != 0)
		{
			int shift = exp_diff >> 11;
			if (bx != 0)
			{
				b = (ushort)(((b & 2047) | 2048) >> shift);
			}
			else
			{
				b >>= shift - 1;
			}
		}
		else
		{
			if (bx == 0)
			{
				ushort res = (ushort)((a - b) >> 1);
				if (res == 0)
				{
					return (short)res;
				}

				return (short)(res | sign);
			}
			else
			{
				b = (ushort)((b & 2047) | 2048);
			}
		}

		ushort r = (ushort)(a - b);
		if ((r & 0xF800) == exp_part)
		{
			return (short)((r >> 1) | sign);
		}

		ushort am = (ushort)((a & 2047) | 2048);
		ushort new_m = (ushort)(am - b);
		if (new_m == 0)
		{
			return 0;
		}

		while (exp_part != 0 && (new_m & 2048) == 0)
		{
			exp_part -= 0x800;
			if (exp_part != 0)
			{
				new_m <<= 1;
			}
		}

		return (short)((((new_m & 2047) | exp_part) >> 1) | sign);
	}

	private static short f16_mul(short a, short b)
	{
		int sign = (a ^ b) & SignMask;

		if (GetIsInvalid(a) || GetIsInvalid(b))
		{
			if (GetIsNaN(a) || GetIsNaN(b) || GetIsZero(a) || GetIsZero(b))
			{
				return NaNRawValue;
			}

			return (short)(sign | ExponentMask);
		}

		if (GetIsZero(a) || GetIsZero(b))
		{
			return 0;
		}

		ushort m1 = (ushort)GetMantissa(a), m2 = (ushort)GetMantissa(b);
		uint v = m1;
		v *= m2;
		int ax = GetExponent(a), bx = GetExponent(b);
		ax += ax == 0 ? 1 : 0;
		bx += bx == 0 ? 1 : 0;
		int new_exp = ax + bx - 15;

		if ((v & ((uint)1 << 21)) != 0)
		{
			v >>= 11;
			new_exp++;
		}
		else if ((v & ((uint)1 << 20)) != 0)
		{
			v >>= 10;
		}
		else
		{
			// Denormal.
			new_exp -= 10;
			while (v >= 2048)
			{
				v >>= 1;
				new_exp++;
			}
		}
		if (new_exp <= 0)
		{
			v >>= -new_exp + 1;
			new_exp = 0;
		}
		else if (new_exp >= 31)
		{
			return GetSignedInfinity((short)sign);
		}

		return (short)((short)((short)sign | (short)(new_exp << 10)) | (short)(v & 1023));
	}

	private static short f16_div(short a, short b)
	{
		short sign = (short)((a ^ b) & SignMask);
		if (GetIsNaN(a) || GetIsNaN(b) || GetIsInvalid(a) && GetIsInvalid(b) || GetIsZero(a) && GetIsZero(b))
		{
			return NaNRawValue;
		}

		if (GetIsZero(a) || GetIsZero(b))
		{
			return (short)(sign | ExponentMask);
		}

		if (GetIsInvalid(b))
		{
			return 0;
		}

		if (GetIsZero(a))
		{
			return 0;
		}

		ushort m1 = (ushort)GetMantissa(a), m2 = (ushort)GetMantissa(b);
		uint m1_shifted = m1;
		m1_shifted <<= 10;
		uint v = m1_shifted / m2;
		ushort rem = (ushort)(m1_shifted % m2);

		int ax = GetExponent(a), bx = GetExponent(b);
		ax += ax == 0 ? 1 : 0;
		bx += bx == 0 ? 1 : 0;
		int new_exp = ax - bx + 15;

		if (v == 0 && rem == 0)
		{
			return 0;
		}

		while (v < 1024 && new_exp > 0)
		{
			v <<= 1;
			rem <<= 1;
			if (rem >= m2)
			{
				v++;
				rem -= m2;
			}

			new_exp--;
		}
		while (v >= 2048)
		{
			v >>= 1;
			new_exp++;
		}

		if (new_exp <= 0)
		{
			v >>= (-new_exp + 1);
			new_exp = 0;
		}
		else if (new_exp >= 31)
		{
			return GetSignedInfinity(sign);
		}

		return (short)((short)(sign | (short)(v & 1023)) | (short)(new_exp << 10));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool f16_gte(short a, short b)
	{
		if (GetIsZero(a) && GetIsZero(b))
		{
			return true;
		}

		if (GetIsNaN(a) || GetIsNaN(b))
		{
			return false;
		}

		return (a & SignMask) == 0
			? (b & SignMask) == SignMask || a >= b
			: (b & SignMask) != 0 && (a & 0x7FFF) <= (b & 0x7FFF);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool f16_gt(short a, short b) =>
		!GetIsNaN(a) && !GetIsNaN(b) && (!GetIsZero(a) || !GetIsZero(b)) && ((a & SignMask) == 0
			? (b & SignMask) == SignMask || a > b
			: (b & SignMask) != 0 && (a & NaNRawValue) < (b & NaNRawValue));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool f16_eq(short a, short b) =>
		!GetIsNaN(a) && !GetIsNaN(b) && (GetIsZero(a) && GetIsZero(b) || a == b);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float16_t operator -(float16_t @this) => new((short)(SignMask ^ @this._mask));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float16_t operator +(float16_t left, float16_t right) => new(f16_add(left._mask, right._mask));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float16_t operator -(float16_t left, float16_t right) => new(f16_sub(left._mask, right._mask));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float16_t operator *(float16_t left, float16_t right) => new(f16_mul(left._mask, right._mask));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float16_t operator /(float16_t left, float16_t right) => new(f16_div(left._mask, right._mask));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >=(float16_t left, float16_t right) => f16_gte(left._mask, right._mask);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <=(float16_t left, float16_t right)
	{
		short lMask = left._mask, rMask = right._mask;
		return !GetIsNaN(lMask) && !GetIsNaN(rMask) && f16_gte(rMask, lMask);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >(float16_t left, float16_t right) => f16_gt(left._mask, right._mask);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <(float16_t left, float16_t right)
	{
		short lMask = left._mask, rMask = right._mask;
		return !GetIsNaN(lMask) && !GetIsNaN(rMask) && f16_gt(rMask, lMask);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(float16_t left, float16_t right) => f16_eq(left._mask, right._mask);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(float16_t left, float16_t right) => !f16_eq(left._mask, right._mask);


	/// <inheritdoc/>
	public static implicit operator float16_t(int value)
	{
		uint v;
		int sig = 0;
		if (value < 0)
		{
			v = (uint)-value;
			sig = 1;
		}
		else
		{
			v = (uint)value;
		}

		if (v == 0)
		{
			return 0;
		}

		int e = 25;
		while (v >= 2048)
		{
			v >>= 1;
			e++;
		}
		while (v < 1024)
		{
			v <<= 1;
			e--;
		}

		if (e >= 31)
		{
			return new(GetSignedInfinity((short)(sig << 15)));
		}

		return new((short)((short)((short)(sig << 15) | (short)(e << 10)) | (short)(v & 1023)));
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator int(float16_t z)
	{
		short a = z._mask;
		ushort value = (ushort)GetMantissa(a);
		short shift = (short)(GetExponent(a) - 25);
		if (shift > 0)
		{
			value <<= shift;
		}
		else if (shift < 0)
		{
			value >>= -shift;
		}

		if ((a & SignMask) != 0)
		{
			return -value;
		}

		return value;
	}
}
