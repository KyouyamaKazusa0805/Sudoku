namespace Sudoku.Concepts.ObjectModel;

/// <summary>
/// Represents for a house blossom branch collection.
/// </summary>
[Equals]
[EqualityOperators]
public sealed partial class HouseBlossomBranchCollection :
	DeathBlossomBranchCollection<HouseBlossomBranchCollection, Cell>,
	IEqualityOperators<HouseBlossomBranchCollection, HouseBlossomBranchCollection, bool>
{
	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] HouseBlossomBranchCollection? other)
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
}
