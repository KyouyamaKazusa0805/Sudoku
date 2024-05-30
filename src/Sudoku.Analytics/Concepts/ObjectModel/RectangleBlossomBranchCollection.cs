namespace Sudoku.Concepts.ObjectModel;

/// <summary>
/// Represents for a rectangle blossom branch collection.
/// </summary>
[EqualityOperators]
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.EqualityOperators)]
public sealed partial class RectangleBlossomBranchCollection :
	DeathBlossomBranchCollection<RectangleBlossomBranchCollection, Candidate>,
	IEqualityOperators<RectangleBlossomBranchCollection, RectangleBlossomBranchCollection, bool>
{
	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] RectangleBlossomBranchCollection? other)
	{
		if (other is null)
		{
			return false;
		}

		var thisCandidates = (CandidateMap)([.. Keys]);
		var otherCandidates = (CandidateMap)([.. other.Keys]);
		if (thisCandidates != otherCandidates)
		{
			return false;
		}

		foreach (var candidate in thisCandidates)
		{
			var thisAls = this[candidate];
			var otherAls = other[candidate];
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
