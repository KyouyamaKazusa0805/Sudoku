using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.SourceGeneration;

namespace Sudoku.Concepts.ObjectModel;

/// <summary>
/// Represents for a house blossom branch collection.
/// </summary>
[Equals]
[EqualityOperators]
public sealed partial class HouseBlossomBranchCollection :
	Dictionary<Cell, AlmostLockedSet>,
	IEquatable<HouseBlossomBranchCollection>,
	IEqualityOperators<HouseBlossomBranchCollection, HouseBlossomBranchCollection, bool>
{
	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] HouseBlossomBranchCollection? other)
	{
		if (other is null)
		{
			return false;
		}

		var thisCells = (CellMap)([.. Keys]);
		var otherCells = (CellMap)([.. other.Keys]);
		if (thisCells != otherCells)
		{
			return false;
		}

		foreach (var cell in thisCells)
		{
			var thisAls = this[cell];
			var otherAls = other[cell];
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
	public ReadOnlySpan<TResult> Select<TResult>(Func<(Cell Cell, AlmostLockedSet AlsPattern), TResult> selector)
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
