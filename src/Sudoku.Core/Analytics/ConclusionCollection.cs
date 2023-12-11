using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.SourceGeneration;
using Sudoku.Concepts.Parsers;
using Sudoku.Concepts.Primitive;
using Sudoku.Text.Converters;
using Sudoku.Text.Parsers;
using static Sudoku.Analytics.ConclusionType;

namespace Sudoku.Analytics;

/// <summary>
/// Represents a collection of conclusions.
/// </summary>
/// <remarks>
/// This type uses <see cref="BitArray"/> to make determining on equality for two collections of <see cref="Conclusion"/> instances.
/// Because the type contains a reference-typed field, the type is also a reference type.
/// </remarks>
/// <seealso cref="BitArray"/>
/// <seealso cref="Conclusion"/>
[Equals]
[EqualityOperators]
public sealed partial class ConclusionCollection :
	ICoordinateObject<ConclusionCollection>,
	IEnumerable<Conclusion>,
	IEquatable<ConclusionCollection>,
	IEqualityOperators<ConclusionCollection, ConclusionCollection, bool>,
	ISimpleParsable<ConclusionCollection>
{
	/// <summary>
	/// The total length of bits.
	/// </summary>
	private const int BitsCount = 729 << 1;


	/// <summary>
	/// The prime numbers below 100.
	/// </summary>
	private static readonly int[] PrimeNumbers = [2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97];


	/// <summary>
	/// The internal bit array.
	/// </summary>
	private readonly BitArray _bitArray = new(BitsCount);

	/// <summary>
	/// The entry point that can visit conclusions.
	/// </summary>
	private readonly List<Conclusion> _conclusionsEntry = [];


	/// <summary>
	/// Indicates the number of bit array elements.
	/// </summary>
	public int Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _conclusionsEntry.Count;
	}


	/// <summary>
	/// An empty instance.
	/// </summary>
	public static ConclusionCollection Empty => [];


	/// <summary>
	/// Try to get n-th element stored in the collection.
	/// </summary>
	/// <param name="index">The desired index to be checked.</param>
	/// <returns>The found <see cref="Conclusion"/> instance at the specified index.</returns>
	/// <exception cref="IndexOutOfRangeException">Throws when the index is out of range.</exception>
	/// <exception cref="InvalidOperationException">Throws when the specified element is not found.</exception>
	public Conclusion this[int index]
	{
		get
		{
			if (index < 0 || index >= Count)
			{
				throw new IndexOutOfRangeException();
			}

			var p = -1;
			for (var i = 0; i < BitsCount; i++)
			{
				if (_bitArray[i] && ++p == index)
				{
					return new(i > 729 ? Elimination : Assignment, i % 729 / 9, i % 729 % 9);
				}
			}

			throw new InvalidOperationException("The element at the specified index is not found.");
		}
	}


	/// <summary>
	/// Add a new element into the collection.
	/// </summary>
	/// <param name="conclusion">The conclusion to be added.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(Conclusion conclusion)
	{
		var (type, cell, digit) = conclusion;
		_bitArray[(int)type * 729 + cell * 9 + digit] = true;
		_conclusionsEntry.Add(conclusion);
	}

	/// <summary>
	/// Add a list of conclusions into the collection.
	/// </summary>
	/// <param name="conclusions">The conclusions to be added.</param>
	public void AddRange(scoped ReadOnlySpan<Conclusion> conclusions)
	{
		foreach (var conclusion in conclusions)
		{
			Add(conclusion);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] ConclusionCollection? other)
	{
		if (other is null)
		{
			return false;
		}

		for (var i = 0; i < BitsCount; i++)
		{
			if (_bitArray[i] != other._bitArray[i])
			{
				return false;
			}
		}

		return true;
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var result = new HashCode();
		var i = 0;
		foreach (bool element in _bitArray)
		{
			if (element)
			{
				result.Add(PrimeNumbers[i % PrimeNumbers.Length] * i);
			}

			i++;
		}

		return result.ToHashCode();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => new RxCyConverter().ConclusionConverter([.. _conclusionsEntry]);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(CoordinateConverter converter) => converter.ConclusionConverter([.. _conclusionsEntry]);

	/// <summary>
	/// Try to get an enumerator type that iterates on each conclusion.
	/// </summary>
	/// <returns>An enumerator type that iterates on each conclusion.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(this);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Conclusion>)this).GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<Conclusion> IEnumerable<Conclusion>.GetEnumerator() => _conclusionsEntry.GetEnumerator();


	/// <inheritdoc/>
	public static bool TryParse(string str, [NotNullWhen(true)] out ConclusionCollection? result)
	{
		try
		{
			result = Parse(str);
			return true;
		}
		catch (FormatException)
		{
			result = null;
			return false;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ConclusionCollection Parse(string str) => [.. new RxCyParser().ConclusionParser(str)];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ConclusionCollection ParseExact(string str, CoordinateParser parser) => [.. parser.ConclusionParser(str)];
}
