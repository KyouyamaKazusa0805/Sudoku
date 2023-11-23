using System.Runtime.CompilerServices;
using System.SourceGeneration;

namespace Sudoku.Analytics.Rating;

/// <summary>
/// Represents the methods that calculates for distance.
/// </summary>
/// <param name="p">Indicates the integer part.</param>
/// <param name="q">Indicates the root part.</param>
/// <exception cref="ArgumentOutOfRangeException">Throws when either <paramref name="p"/> or <paramref name="q"/> are less than 1.</exception>
/// <remarks>
/// This type is implemented via irrational numbers logic that only takes a square root.
/// </remarks>
[Equals]
[GetHashCode]
[EqualityOperators]
[ComparisonOperators]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly ref partial struct Distance(int p, int q)
{
	/// <summary>
	/// The root value of P.
	/// </summary>
	private readonly int _p = p < 1
		? throw new ArgumentOutOfRangeException(nameof(p))
		: q < 1
			? throw new ArgumentOutOfRangeException(nameof(q))
			: p * SimplifyRootPart(ref q);

	/// <summary>
	/// The root value of Q.
	/// </summary>
	private readonly int _q = q;


	/// <summary>
	/// Initializes a <see cref="Distance"/> instance via both values 1.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Distance() : this(1, 1)
	{
	}

	/// <summary>
	/// Initializes a <see cref="Distance"/> instance via the root part, with the default integer part 1.
	/// <i>This value will automatically simplify the root expression, e.g. sqrt(18) -> 3sqrt(2).</i>
	/// </summary>
	/// <param name="q">The root value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Distance(int q) : this(1, q)
	{
	}


	/// <summary>
	/// The raw value of the distance. The value will be ouput as a <see cref="double"/> value.
	/// </summary>
	public double RawValue => _p * Math.Sqrt(_q);


	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Distance other) => (_p, _q) == (other._p, other._q);

	/// <inheritdoc cref="IComparable{T}.CompareTo(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Distance other) => RawValue.CompareTo(other.RawValue);

	/// <inheritdoc cref="object.ToString"/>
	/// <remarks>
	/// The output format will be "<c>psq</c>", where <c>p</c> and <c>q</c> are the variables, and <c>s</c> means "square root of".
	/// For example, "<c>3s2</c>" means <c>3 * sqrt(2)</c>, i.e. <c>sqrt(18)</c>.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => (_p, _q) switch { (_, 1) => _p.ToString(), (1, _) => $"s{_q}", _ => $"{_p}s{_q}" };


	/// <summary>
	/// Try to fetch the distance for the two cells.
	/// </summary>
	/// <param name="cell1">The first cell to be compared.</param>
	/// <param name="cell2">The second cell to be compared.</param>
	/// <returns>The distance result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Distance GetDistanceFor(Cell cell1, Cell cell2)
	{
		var (x1, y1) = (cell1 / 9, cell1 % 9);
		var (x2, y2) = (cell2 / 9, cell2 % 9);
		return new((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
	}

	/// <summary>
	/// Simplifies for root part.
	/// </summary>
	/// <param name="base">The root value.</param>
	/// <returns>The P value.</returns>
	private static int SimplifyRootPart(scoped ref int @base)
	{
		var result = 1;
		var temp = @base;
		for (var i = 2; i * i <= @base;)
		{
			if (temp % (i * i) == 0)
			{
				temp /= i * i;
				result *= i;
				continue;
			}

			i = i == 2 ? 3 : i + 2;
		}

		@base = temp;
		return result;
	}


	/// <summary>
	/// Implicit cast from the <see cref="Distance"/> instance to a <see cref="double"/>.
	/// </summary>
	/// <param name="distance">The distance value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator double(Distance distance) => distance.RawValue;
}
