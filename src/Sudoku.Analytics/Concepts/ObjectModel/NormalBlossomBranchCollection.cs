namespace Sudoku.Concepts.ObjectModel;

/// <summary>
/// Represents for a normal blossom branch collection.
/// </summary>
[EqualityOperators]
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.EqualityOperators)]
public sealed partial class NormalBlossomBranchCollection :
	DeathBlossomBranchCollection<NormalBlossomBranchCollection, Digit>,
	IEqualityOperators<NormalBlossomBranchCollection, NormalBlossomBranchCollection, bool>
{
	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] NormalBlossomBranchCollection? other)
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
}
