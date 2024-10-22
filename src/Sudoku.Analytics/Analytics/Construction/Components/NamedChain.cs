namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents named chain patterns:
/// <list type="bullet">
/// <item>Alternating inference chain (corresponds to type <see cref="AlternatingInferenceChain"/>)</item>
/// <item>Continuous nice loop (corresponds to type <see cref="ContinuousNiceLoop"/>)</item>
/// </list>
/// </summary>
/// <param name="lastNode"><inheritdoc cref="Chain(Node, bool, bool)" path="/param[@name='lastNode']"/></param>
/// <param name="isLoop"><inheritdoc cref="Chain(Node, bool, bool)" path="/param[@name='isLoop']"/></param>
/// <seealso cref="AlternatingInferenceChain"/>
/// <seealso cref="ContinuousNiceLoop"/>
public abstract class NamedChain(Node lastNode, bool isLoop) : Chain(lastNode, isLoop, true)
{
	/// <inheritdoc/>
	public sealed override bool IsNamed => true;

	/// <summary>
	/// Indicates whether the chain is ALS-XZ, ALS-XY-Wing or ALS-XY-Chain.
	/// </summary>
	public bool IsAlmostLockedSetSequence
	{
		get
		{
			// A valid ALS chain-like pattern should consider two conditions:
			//   1) All strong links use ALS
			//   2) All weak links use RCC (a weak link to connect two same digit)
			foreach (var link in Links)
			{
				switch (link)
				{
					case { IsStrong: true, GroupedLinkPattern: not AlmostLockedSetPattern }:
					case { IsStrong: false, GroupedLinkPattern: not null }:
					case { IsStrong: false, FirstNode.Map.Digits: var d1, SecondNode.Map.Digits: var d2 }
					when d1 != d2 || !Mask.IsPow2(d1):
					{
						return false;
					}
				}
			}
			return true;
		}
	}

	/// <summary>
	/// Indicates the number of grouped pattern used in chain.
	/// </summary>
	public FrozenDictionary<PatternType, int> GroupedPatternsCount
	{
		get
		{
			var result = new Dictionary<PatternType, int>();
			foreach (var link in Links)
			{
				if (link.GroupedLinkPattern is { } pattern)
				{
					if (!result.TryAdd(pattern.Type, 1))
					{
						result[pattern.Type]++;
					}
				}
			}
			return result.ToFrozenDictionary();
		}
	}

	/// <summary>
	/// Indicates the number of ALSes used in chain.
	/// </summary>
	internal int AlmostLockedSetsCount => GroupedPatternsCount.TryGetValue(PatternType.AlmostLockedSet, out var r) ? r : 0;


	/// <summary>
	/// Try to get a <see cref="ConclusionSet"/> instance that contains all conclusions created by using the current chain.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <returns>A <see cref="ConclusionSet"/> instance. By default the method returns an empty conclusion set.</returns>
	public virtual ConclusionSet GetConclusions(ref readonly Grid grid) => ConclusionSet.Empty;
}
