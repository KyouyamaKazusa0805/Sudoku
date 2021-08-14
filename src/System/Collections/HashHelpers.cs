namespace System.Collections;

/// <summary>
/// Encapsulates and provides the methods about hash code handling.
/// </summary>
internal static class HashHelpers
{
	/// <summary>
	/// Indicates the threshold on the hash collision case.
	/// </summary>
	public const uint HashCollisionThreshold = 100;

	/// <summary>
	/// Indiactes the maximum prime number of the type <see cref="int"/>
	/// smaller than <see cref="Array.MaxLength"/>..
	/// </summary>
	/// <seealso cref="Array.MaxLength"/>
	/// <remarks>
	/// The next prime is <c>2147483629</c>, which is bigger than <see cref="Array.MaxLength"/>
	/// though it is still in the valid range of <see cref="int"/> type.
	/// </remarks>
	public const int MaxPrimeArrayLength = 2147483587;

	/// <summary>
	/// Indicates the hash prime number used.
	/// </summary>
	public const int HashPrime = 101;


	/// <summary>
	/// <para>
	/// Table of prime numbers to use as hash table sizes.
	/// A typical resize algorithm would pick the smallest prime number in this array
	/// that is larger than twice the previous capacity.
	/// </para>
	/// <para>
	/// Suppose our Hashtable currently has capacity <c>x</c> and enough elements are added
	/// such that a resize needs to occur. Resizing first computes <c>2x</c> then finds the
	/// first prime in the table greater than <c>2x</c>, i.e. if primes are ordered
	/// <c>p_1, p_2, ..., p_i, ...</c>, it finds <c>p_n</c> such that <c><![CDATA[p_n-1 < 2x < p_n]]></c>.
	/// Doubling is important for preserving the asymptotic complexity of the
	/// hashtable operations such as add.  Having a prime guarantees that double
	/// hashing does not lead to infinite loops.  IE, your hash function will be
	/// <c><![CDATA[h1(key) + i * h2(key)]]></c>, <c><![CDATA[0 <= i < size]]></c>.
	/// <c>h2</c> and the size must be relatively prime.
	/// </para>
	/// <para>
	/// We prefer the low computation costs of higher prime numbers over the increased
	/// memory allocation of a fixed prime number i.e. when right sizing a <c>HashSet</c>.
	/// </para>
	/// </summary>
	private static readonly int[] Primes =
	{
			3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
			1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
			17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
			187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
			1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369
		};


	/// <summary>
	/// Checks whether the specified candidate number is a prime number using the normal algorithm.
	/// </summary>
	/// <param name="candidate">The number to check.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool IsPrime(int candidate)
	{
		if ((candidate & 1) != 0)
		{
			int limit = (int)Sqrt(candidate);
			for (int divisor = 3; divisor <= limit; divisor += 2)
			{
				if (candidate % divisor == 0)
				{
					return false;
				}
			}

			return true;
		}

		return candidate == 2;
	}

	/// <summary>
	/// Gets a prime number.
	/// </summary>
	/// <param name="threshold">
	/// The minimum value as the threshold. The return value will be greater than this value.
	/// </param>
	/// <returns>The result prime number.</returns>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="threshold"/> is a negative value.
	/// </exception>
	public static int GetPrime(int threshold)
	{
		if (threshold < 0)
		{
			throw new ArgumentException("You can't assign the value as a negative one.", nameof(threshold));
		}

		foreach (int prime in Primes)
		{
			if (prime >= threshold)
			{
				return prime;
			}
		}

		// Outside of our predefined table. Compute the hard way.
		for (int i = threshold | 1; i < int.MaxValue; i += 2)
		{
			if (IsPrime(i) && (i - 1) % HashPrime != 0)
			{
				return i;
			}
		}

		return threshold;
	}

	/// <summary>
	/// Expand the prime number from an old size.
	/// </summary>
	/// <param name="oldSize">The old size.</param>
	/// <returns>Returns size of hashtable to grow to.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ExpandPrime(int oldSize) =>
		// Allow the hashtables to grow to maximum possible size (~2G elements)
		// before encountering capacity overflow.
		// Note that this check works even when '_items.Length' overflowed thanks to the (uint) cast.
		2 * oldSize is var newSize && (uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize
			? MaxPrimeArrayLength
			: GetPrime(newSize);

	/// <summary>
	/// Returns approximate reciprocal of the divisor: <c>Ceil(Pow(2, 64) / divisor)</c>.
	/// </summary>
	/// <remarks>This should only be used on 64-bit.</remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong GetFastModMultiplier(uint divisor) => ulong.MaxValue / divisor + 1;

	/// <summary>
	/// Performs a mod operation using the multiplier pre-computed with <see cref="GetFastModMultiplier"/>.
	/// </summary>
	/// <remarks>This should only be used on 64-bit.</remarks>
	/// <exception cref="ArgumentException">
	/// Throws when the <paramref name="divisor"/> is greater than <see cref="int.MaxValue"/>.
	/// </exception>
	/// <exception cref="ArithmeticException">
	/// Throws when the expression <c>highbits == value % divisor</c> returns <see langword="false"/>,
	/// where the variable <c>highbits</c> equals to the expression
	/// <c>(uint)(((((multiplier * value) >> 32) + 1) * divisor) >> 32)</c>,
	/// which is an <see cref="uint"/> value.
	/// </exception>
	/// <seealso cref="GetFastModMultiplier"/>
	/// <seealso cref="int.MaxValue"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint FastMod(uint value, uint divisor, ulong multiplier)
	{
		// We use modified Daniel Lemire's fastmod algorithm (https://github.com/dotnet/runtime/pull/406),
		// which allows to avoid the long multiplication if the divisor is less than '1 << 31'.
		if (divisor > int.MaxValue)
		{
			throw new ArgumentException(
				"The divisor can't be greater than int.MaxValue (2147483647) even though it's an uint value.",
				nameof(divisor)
			);
		}

		// This is equivalent of '(uint)Math.BigMul(multiplier * value, divisor, out _)'.
		// This version is faster than BigMul currently because we only need the high bits.
		uint highbits = (uint)(((((multiplier * value) >> 32) + 1) * divisor) >> 32);
		if (highbits != value % divisor)
		{
			throw new ArithmeticException("The operation handled wrong.");
		}

		return highbits;
	}
}
