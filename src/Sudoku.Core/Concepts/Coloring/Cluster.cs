namespace Sudoku.Concepts.Coloring;

using ConflictedInfo = ((Cell Left, Cell Right), CellMap InfluencedRange);

/// <summary>
/// <para>Represents a cluster. A cluster is a group of candidates which are all connected with strong links.</para>
/// <para>
/// This data structure will simplify the definition, only reserving for single-digit strong links (i.e. conjugate pairs),
/// in order to make the implementation behave well and easily on representing data.
/// </para>
/// <para>
/// Please visit <see href="http://sudopedia.enjoysudoku.com/Cluster.html">this link</see> to learn more information.
/// </para>
/// </summary>
/// <param name="grid">Indicates the grid used.</param>
/// <param name="digit">Indicates the digit used.</param>
/// <param name="map">Indicates the cells used.</param>
/// <seealso href="http://sudopedia.enjoysudoku.com/Cluster.html">Sudopedia Mirror - Cluster</seealso>
[StructLayout(LayoutKind.Auto)]
[TypeImpl(
	TypeImplFlag.AllObjectMethods | TypeImplFlag.EqualityOperators | TypeImplFlag.Equatable,
	ToStringBehavior = ToStringBehavior.RecordLike)]
public readonly ref partial struct Cluster(
	[Field, HashCodeMember, StringMember] ref readonly Grid grid,
	[Property, HashCodeMember, StringMember] Digit digit,
	[Field(RefKind = null), HashCodeMember, StringMember] scoped ref readonly CellMap map
) : IEquatable<Cluster>
{
	/// <summary>
	/// Indicates the internal map.
	/// </summary>
	[UnscopedRef]
	[EquatableMember]
	public ref readonly CellMap Map => ref _map;

	/// <summary>
	/// Represents a list of cells that will form wrap contradictions in the cluster.
	/// </summary>
	public CellMap WrapContradictions
	{
		get
		{
			var result = CellMap.Empty;
			var graph = CellGraph.CreateFromConjugatePair(in _grid, Digit, in Map);
			foreach (ref readonly var component in graph.Components)
			{
				var parities = Parity.Create(in component);
				if (parities.Length == 0)
				{
					continue;
				}

				ref readonly var firstParityPair = ref parities[0];
				var parity1 = firstParityPair.On.Cells;
				var parity2 = firstParityPair.Off.Cells;
				for (var i = 0; i < 2; i++)
				{
					// Check whether there're two or more cells lying in a same house.
					ref readonly var parity = ref i == 0 ? ref parity1 : ref parity2;
					foreach (var house in parity.Houses)
					{
						if ((HousesMap[house] & parity).Count >= 2)
						{
							// All this parity is incorrect, and the other one is correct.
							result |= parity;
							goto NextComponent;
						}
					}
				}

			NextComponent:
				;
			}
			return result;
		}
	}

	/// <summary>
	/// Represents a list of cells that will form trap contradictions in the cluster.
	/// </summary>
	public CellMap TrapContradictions
	{
		get
		{
			var candsMap = _grid.CandidatesMap[Digit];
			var result = CellMap.Empty;
			var graph = CellGraph.CreateFromConjugatePair(in _grid, Digit, in Map);
			foreach (ref readonly var component in graph.Components)
			{
				var parities = Parity.Create(in component);
				if (parities.Length == 0)
				{
					continue;
				}

				ref readonly var firstParityPair = ref parities[0];
				var parity1 = firstParityPair.On.Cells;
				var parity2 = firstParityPair.Off.Cells;

				// Now we should iterate two collections to get contradiction.
				var (conflictedCells, conflictedPair) = (CellMap.Empty, new HashSet<ConflictedInfo>());
				foreach (var cell1 in parity1)
				{
					foreach (var cell2 in parity2)
					{
						var intersection = (cell1.AsCellMap() + cell2).PeerIntersection;
						var currentConflictCells = intersection & candsMap;
						if (!!currentConflictCells
							&& !conflictedPair.Any(p => (p.InfluencedRange & currentConflictCells) == currentConflictCells))
						{
							conflictedPair.Add(((cell1, cell2), currentConflictCells));
							conflictedCells |= currentConflictCells;
						}
					}
				}
				result |= conflictedCells;
			}
			return result;
		}
	}


	/// <summary>
	/// Creates a <see cref="Cluster"/> instance via the specified grid.
	/// </summary>
	/// <param name="grid">The grid to be used.</param>
	/// <param name="digit">Indicates the digits used.</param>
	/// <returns>A <see cref="Cluster"/> instance.</returns>
	public static Cluster Create(ref readonly Grid grid, Digit digit)
	{
		var result = CellMap.Empty;
		foreach (var cp in grid.ConjugatePairs)
		{
			if (cp.Digit == digit)
			{
				result |= cp.Map;
			}
		}
		return new(in grid, digit, in result);
	}
}
