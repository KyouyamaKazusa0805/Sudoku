using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.SourceGeneration;

namespace Sudoku.Concepts.ObjectModel;

/// <summary>
/// Represents for a blossom branch.
/// </summary>
[Equals]
[EqualityOperators]
public sealed partial class BlossomBranch :
	Dictionary<Digit, AlmostLockedSet>,
	IEquatable<BlossomBranch>,
	IEqualityOperators<BlossomBranch, BlossomBranch, bool>
{
	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] BlossomBranch? other)
	{
		if (other is null)
		{
			return false;
		}

		var thisMask = (Mask)0;
		var otherMask = (Mask)0;
		foreach (var key in Keys)
		{
			thisMask |= (Mask)(1 << key);
		}
		foreach (var key in other.Keys)
		{
			otherMask |= (Mask)(1 << key);
		}
		if (thisMask != otherMask)
		{
			return false;
		}

		foreach (var digit in thisMask)
		{
			var thisAls = this[digit];
			var otherAls = other[digit];
			if (thisAls != otherAls)
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
		foreach (var (key, value) in this)
		{
			result.Add(key << 17 | 135792468);
			result.Add(value.GetHashCode());
		}

		return result.ToHashCode();
	}

	/// <summary>
	/// Transforms the current collection into another representation, using the specified function to transform.
	/// </summary>
	/// <typeparam name="TResult">The type of the results.</typeparam>
	/// <param name="selector">The selector to tranform elements.</param>
	/// <returns>The results.</returns>
	public ReadOnlySpan<TResult> Select<TResult>(Func<(Digit Digit, AlmostLockedSet AlsPattern), TResult> selector)
	{
		var result = new TResult[Count];
		var i = 0;
		foreach (var (key, value) in this)
		{
			result[i++] = selector((key, value));
		}

		return result;
	}
}
