namespace Sudoku.Solving.Logical.PatternData;

/// <summary>
/// Defines a full chain.
/// </summary>
internal sealed partial class FullChain : IEquatable<FullChain>, IEqualityOperators<FullChain, FullChain, bool>
{
	/// <summary>
	/// Initializes a <see cref="FullChain"/> instance via the specified <see cref="SudokuExplainerCompatibleChainStep"/> instance.
	/// </summary>
	/// <param name="chain">The step.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FullChain(SudokuExplainerCompatibleChainStep chain) => Step = chain;


	/// <summary>
	/// Indicates the step used.
	/// </summary>
	public SudokuExplainerCompatibleChainStep Step { get; }


	[GeneratedOverriddingMember(GeneratedEqualsBehavior.AsCastAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] FullChain? other)
	{
		if (other is null)
		{
			return false;
		}

		var thisTargets = Step.GetChainsTargets().ToArray();
		var otherTargets = other.Step.GetChainsTargets().ToArray();
		if (!MemoryExtensions.SequenceEqual<Potential>(thisTargets, otherTargets))
		{
			return false;
		}

		var i1 = ((IEnumerable<Potential>)thisTargets).GetEnumerator();
		var i2 = ((IEnumerable<Potential>)otherTargets).GetEnumerator();
		while (i1.MoveNext() && i2.MoveNext())
		{
			if (!Step.GetChain(i1.Current).SequenceEqual(other.Step.GetChain(i2.Current)))
			{
				return false;
			}
		}

		return true;
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var result = 0;
		foreach (var target in Step.GetChainsTargets())
		{
			foreach (var p in Step.GetChain(target))
			{
				result ^= p.GetHashCode();
			}
		}

		return result;
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(FullChain? left, FullChain? right)
		=> (left, right) switch { (null, null) => true, (not null, not null) => left.Equals(right), _ => false };

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(FullChain? left, FullChain? right) => !(left == right);
}
