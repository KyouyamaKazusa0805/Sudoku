namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents for a n-times ALS blossom branch collection.
/// </summary>
[TypeImpl(TypeImplFlags.Object_Equals | TypeImplFlags.EqualityOperators)]
public sealed partial class NTimesAlmostLockedSetsBlossomBranchCollection :
	DeathBlossomBranchCollection<NTimesAlmostLockedSetsBlossomBranchCollection, CandidateMap>,
	IEqualityOperators<NTimesAlmostLockedSetsBlossomBranchCollection, NTimesAlmostLockedSetsBlossomBranchCollection, bool>
{
	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] NTimesAlmostLockedSetsBlossomBranchCollection? other)
	{
		if (other is null)
		{
			return false;
		}

		var thisKeys = (CandidateMap[])[.. Keys];
		var otherKeys = (CandidateMap[])[.. other.Keys];
		if (thisKeys.Length != otherKeys.Length)
		{
			return false;
		}

		for (var i = 0; i < thisKeys.Length; i++)
		{
			if (thisKeys[i] != otherKeys[i])
			{
				return false;
			}
		}

		foreach (var candidatesMap in thisKeys)
		{
			var thisAls = this[candidatesMap];
			var otherAls = other[candidatesMap];
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
		var result = default(HashCode);
		foreach (var (key, value) in this)
		{
			result.Add(key.GetHashCode() << 17 | 135792468);
			result.Add(value.GetHashCode());
		}

		return result.ToHashCode();
	}
}
