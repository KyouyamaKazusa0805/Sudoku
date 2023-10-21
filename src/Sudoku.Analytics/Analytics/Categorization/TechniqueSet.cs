using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.SourceGeneration;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Represents a set of <see cref="Technique"/> fields.
/// </summary>
/// <remarks>
/// This type uses a <see cref="BitArray"/> to construct the data structure. Because <see cref="BitArray"/> is a reference type,
/// we cannot directly copy a <see cref="TechniqueSet"/> value. If you want to clone a <see cref="TechniqueSet"/>,
/// please use copy constructor <see cref="TechniqueSet(TechniqueSet)"/>.
/// </remarks>
/// <seealso cref="Technique"/>
/// <seealso cref="BitArray"/>
/// <seealso cref="TechniqueSet(TechniqueSet)"/>
/// <completionlist cref="TechniqueSets"/>
[JsonConverter(typeof(Converter))]
[Equals]
[EqualityOperators]
public partial struct TechniqueSet :
	IAdditionOperators<TechniqueSet, Technique, TechniqueSet>,
	IAdditionOperators<TechniqueSet, TechniqueGroup, TechniqueSet>,
	IBitwiseOperators<TechniqueSet, TechniqueSet, TechniqueSet>,
	ICollection<Technique>,
	IEnumerable<Technique>,
	IEquatable<TechniqueSet>,
	IEqualityOperators<TechniqueSet, TechniqueSet, bool>,
	IReadOnlyCollection<Technique>,
	ISet<Technique>,
	IReadOnlySet<Technique>,
	ISubtractionOperators<TechniqueSet, Technique, TechniqueSet>,
	ISubtractionOperators<TechniqueSet, TechniqueSet, TechniqueSet>
{
	/// <summary>
	/// Indicates the information for the techniques, can lookup the relation via its belonging technique group.
	/// This field will be used in extension method <see cref="TechniqueGroupExtensions.GetTechniques(TechniqueGroup)"/>.
	/// </summary>
	/// <seealso cref="TechniqueGroupExtensions.GetTechniques(TechniqueGroup)"/>
	public static readonly IReadOnlyDictionary<TechniqueGroup, TechniqueSet> TechniqueRelationGroups;

	/// <summary>
	/// Indicates the number of techniques included in this solution.
	/// </summary>
	private static readonly Count TechniquesCount = Enum.GetValues<Technique>().Length;


	/// <summary>
	/// The internal bits to store techniques.
	/// </summary>
	private BitArray _techniqueBits;


	/// <summary>
	/// Initializes a <see cref="TechniqueSet"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TechniqueSet() => _techniqueBits = new(TechniquesCount);

	/// <summary>
	/// Copies a <see cref="TechniqueSet"/> instance, and adds it to the current collection.
	/// </summary>
	/// <param name="other">The other collection to be added.</param>
	public TechniqueSet(TechniqueSet other) : this()
	{
		foreach (var technique in other)
		{
			Add(technique);
		}
	}


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static TechniqueSet()
	{
		var dic = new Dictionary<TechniqueGroup, TechniqueSet>();
		foreach (var technique in Enum.GetValues<Technique>())
		{
			if (technique != Technique.None && technique.TryGetGroup() is { } group && !dic.TryAdd(group, [technique]))
			{
				dic[group].Add(technique);
			}
		}

		TechniqueRelationGroups = dic;
	}


	/// <summary>
	/// Indicates the length of the technique.
	/// </summary>
	public readonly Count Count => _techniqueBits.GetCardinality();

	/// <summary>
	/// Indicates the range of difficulty that the current collection containss.
	/// </summary>
	/// <remarks>
	/// This property returns a list of <see cref="DifficultyLevel"/> flags, merged into one instance.
	/// If you want to get the internal fields of flags the return value contains, use <see langword="foreach"/> loop to iterate them,
	/// or use method <see cref="EnumExtensions.GetAllFlags{T}(T)"/>.
	/// </remarks>
	/// <seealso cref="EnumExtensions.GetAllFlags{T}(T)"/>
	public readonly DifficultyLevel DifficultyRange
	{
		get
		{
			var result = DifficultyLevel.Unknown;
			if (Count == 0)
			{
				return result;
			}

			foreach (var technique in this)
			{
				result |= technique.GetDifficultyLevel();
			}

			return result;
		}
	}

	/// <inheritdoc/>
	readonly bool ICollection<Technique>.IsReadOnly => false;


	/// <summary>
	/// Try to get the <see cref="Technique"/> at the specified index.
	/// </summary>
	/// <param name="index">The index to be checked.</param>
	/// <returns>The found <see cref="Technique"/> instance.</returns>
	/// <exception cref="IndexOutOfRangeException">Throws when the index is out of range.</exception>
	public readonly Technique this[Offset index]
	{
		get
		{
			var i = -1;
			foreach (var technique in this)
			{
				if (++i == index)
				{
					return technique;
				}
			}

			throw new IndexOutOfRangeException();
		}
	}

	/// <summary>
	/// Checks the index of the specified technique.
	/// </summary>
	/// <param name="technique">The technique to be checked.</param>
	/// <returns>The index that the technique is at. If none found, -1.</returns>
	public readonly Offset this[Technique technique]
	{
		get
		{
			var result = 0;
			foreach (var currentTechnique in this)
			{
				if (currentTechnique == technique)
				{
					return result;
				}

				result++;
			}

			return -1;
		}
	}


	/// <inheritdoc/>
	public readonly bool Equals(TechniqueSet other)
	{
		if (Count != other.Count)
		{
			return false;
		}

		foreach (var technique in this)
		{
			if (!other.Contains(technique))
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Determines whether the specified technique is in the collection.
	/// </summary>
	/// <param name="item">The technique.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Contains(Technique item) => _techniqueBits[(Offset)item];

	/// <inheritdoc/>
	public override readonly int GetHashCode()
	{
		var result = 0;
		var flag = false;
		foreach (var technique in this)
		{
			var target = (int)technique.GetGroup() * 1000000 + (Offset)technique;
			result |= flag ? target >> 10 : target;
			result += flag ? 7 : 31;

			flag = !flag;
		}

		return result;
	}

	/// <inheritdoc cref="object.ToString"/>
	public override readonly string ToString()
	{
		var currentCountryOrRegionName = CultureInfo.CurrentCulture.Parent.Name;
		var isCurrentCountryOrRegionUseEnglish = currentCountryOrRegionName.Equals(EnglishLanguage, StringComparison.OrdinalIgnoreCase);
		return string.Join(
			GetString("Comma"),
			from technique in ToArray()
			select isCurrentCountryOrRegionUseEnglish ? $"{technique.GetName()}" : $"{technique.GetName()} ({technique.GetEnglishName()})"
		);
	}

	/// <summary>
	/// Converts the current collection into an array.
	/// </summary>
	/// <returns>The final array converted.</returns>
	public readonly Technique[] ToArray()
	{
		var result = new Technique[Count];
		var i = 0;
		foreach (var technique in this)
		{
			result[i++] = technique;
		}

		return result;
	}

	/// <summary>
	/// Forms a slice out of the current collection starting at a specified index for a specified length.
	/// </summary>
	/// <param name="start"><inheritdoc cref="ReadOnlySpan{T}.Slice(Offset)" path="/param[@name='start']"/></param>
	/// <param name="count"><inheritdoc cref="ReadOnlySpan{T}.Slice(Offset, Count)" path="/param[@name='length']"/></param>
	/// <returns>
	/// A new <see cref="TechniqueSet"/> that consists of all elements of the current collection
	/// from <paramref name="start"/> to the end of the slicing, given by <paramref name="count"/>.
	/// </returns>
	public readonly TechniqueSet Slice(Offset start, Count count)
	{
		var result = new TechniqueSet();
		var i = start - 1;
		foreach (var technique in this)
		{
			if (++i < start + count)
			{
				result.Add(technique);
			}
		}

		return result;
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Enumerator GetEnumerator() => new(_techniqueBits);

	/// <inheritdoc cref="ArrayEnumerable.Select{T, TResult}(T[], Func{T, TResult})"/>
	public readonly ReadOnlySpan<TResult> Select<TResult>(Func<Technique, TResult> selector)
	{
		var result = new TResult[Count];
		var i = 0;
		foreach (var technique in this)
		{
			result[i++] = selector(technique);
		}

		return result;
	}

	/// <summary>
	/// Try to add a new technique.
	/// </summary>
	/// <param name="item">A technique to be added.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the current technique is successfully added.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SuppressMessage("Style", "IDE0251:Make member 'readonly'", Justification = "<Pending>")]
	public bool Add(Technique item)
	{
		if (_techniqueBits[(Offset)item])
		{
			return false;
		}

		_techniqueBits.Set((Offset)item, true);
		return true;
	}

	/// <summary>
	/// Try to remove a technique from the collection.
	/// </summary>
	/// <param name="item">A technique to be removed.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the current technique is successfully removed.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SuppressMessage("Style", "IDE0251:Make member 'readonly'", Justification = "<Pending>")]
	public bool Remove(Technique item)
	{
		if (!_techniqueBits[(Offset)item])
		{
			return false;
		}

		_techniqueBits.Set((Offset)item, false);
		return true;
	}

	/// <summary>
	/// Clears the collection, making all techniques to be removed.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SuppressMessage("Style", "IDE0251:Make member 'readonly'", Justification = "<Pending>")]
	public void Clear() => _techniqueBits.SetAll(false);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly void ICollection<Technique>.CopyTo(Technique[] array, Offset arrayIndex)
		=> Array.Copy(this[arrayIndex..].ToArray(), array, Count - arrayIndex);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool ISet<Technique>.IsProperSubsetOf(IEnumerable<Technique> other) => ((IReadOnlySet<Technique>)this).IsProperSubsetOf(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool ISet<Technique>.IsProperSupersetOf(IEnumerable<Technique> other) => ((IReadOnlySet<Technique>)this).IsProperSupersetOf(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool ISet<Technique>.IsSubsetOf(IEnumerable<Technique> other) => ((IReadOnlySet<Technique>)this).IsSubsetOf(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool ISet<Technique>.IsSupersetOf(IEnumerable<Technique> other) => ((IReadOnlySet<Technique>)this).IsSupersetOf(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool ISet<Technique>.Overlaps(IEnumerable<Technique> other) => ((IReadOnlySet<Technique>)this).Overlaps(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool ISet<Technique>.SetEquals(IEnumerable<Technique> other) => ((IReadOnlySet<Technique>)this).SetEquals(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool IReadOnlySet<Technique>.IsProperSubsetOf(IEnumerable<Technique> other)
	{
		var otherSet = (TechniqueSet)([.. other]);
		return (otherSet & this) == this && this != otherSet;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool IReadOnlySet<Technique>.IsProperSupersetOf(IEnumerable<Technique> other)
	{
		var otherSet = (TechniqueSet)([.. other]);
		return (this & otherSet) == otherSet && this != otherSet;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool IReadOnlySet<Technique>.IsSubsetOf(IEnumerable<Technique> other) => ([.. other] & this) == this;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool IReadOnlySet<Technique>.IsSupersetOf(IEnumerable<Technique> other)
	{
		var otherSet = (TechniqueSet)([.. other]);
		return (this & otherSet) == otherSet;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool IReadOnlySet<Technique>.Overlaps(IEnumerable<Technique> other) => (this & [.. other]).Count != 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool IReadOnlySet<Technique>.SetEquals(IEnumerable<Technique> other) => this == [.. other];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly IEnumerator IEnumerable.GetEnumerator() => _techniqueBits.GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly IEnumerator<Technique> IEnumerable<Technique>.GetEnumerator()
	{
		foreach (Technique technique in _techniqueBits)
		{
			yield return technique;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void ISet<Technique>.ExceptWith(IEnumerable<Technique> other) => this -= [.. other];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void ISet<Technique>.IntersectWith(IEnumerable<Technique> other) => this &= [.. other];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void ISet<Technique>.SymmetricExceptWith(IEnumerable<Technique> other)
	{
		var otherSet = (TechniqueSet)([.. other]);
		this = (this - otherSet) | (otherSet - this);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void ISet<Technique>.UnionWith(IEnumerable<Technique> other) => this |= [.. other];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void ICollection<Technique>.Add(Technique item) => Add(item);

	/// <inheritdoc/>
	[SuppressMessage("Style", "IDE0251:Make member 'readonly'", Justification = "<Pending>")]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void ICollection<Technique>.Clear() => _techniqueBits.SetAll(false);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TechniqueSet operator +(TechniqueSet left, Technique right) => [.. left, right];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TechniqueSet operator +(TechniqueSet left, TechniqueGroup right) => [.. left, .. TechniqueRelationGroups[right]];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TechniqueSet operator -(TechniqueSet left, Technique right)
	{
		var result = new TechniqueSet(left);
		result.Remove(right);
		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TechniqueSet operator -(TechniqueSet left, TechniqueSet right) => left & ~right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TechniqueSet operator ~(TechniqueSet value)
	{
		var result = new TechniqueSet(value);
		result._techniqueBits.Not();
		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TechniqueSet operator &(TechniqueSet left, TechniqueSet right)
	{
		var result = new TechniqueSet(left);
		result._techniqueBits.And(right._techniqueBits);
		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TechniqueSet operator |(TechniqueSet left, TechniqueSet right)
	{
		var result = new TechniqueSet(left);
		result._techniqueBits.Or(right._techniqueBits);
		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TechniqueSet operator ^(TechniqueSet left, TechniqueSet right)
	{
		var result = new TechniqueSet(left);
		result._techniqueBits.Xor(right._techniqueBits);
		return result;
	}
}

/// <summary>
/// Defines a JSON converter for the current type.
/// </summary>
file sealed class Converter : JsonConverter<TechniqueSet>
{
	/// <inheritdoc/>
	public override TechniqueSet Read(scoped ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var (result, isInitialized) = (new TechniqueSet(), false);
		while (reader.Read())
		{
			switch (reader.TokenType)
			{
				case JsonTokenType.StartArray when !isInitialized:
				{
					isInitialized = true;
					break;
				}
				case JsonTokenType.String when reader.GetString() is { } s && Enum.TryParse(s, out Technique technique):
				{
					result.Add(technique);
					break;
				}
				case JsonTokenType.EndArray:
				{
					return result;
				}
				default:
				{
					throw new JsonException();
				}
			}
		}

		throw new JsonException();
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, TechniqueSet value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		foreach (var technique in value)
		{
			writer.WriteStringValue(technique.ToString());
		}
		writer.WriteEndArray();
	}
}
