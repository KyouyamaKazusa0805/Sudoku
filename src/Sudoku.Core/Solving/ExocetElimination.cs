namespace Sudoku.Solving;

/// <summary>
/// Represents a data structure that describes the eliminations that are created and proved
/// by the exocet technique.
/// </summary>
/// <param name="Eliminations">Indicates the eliminations.</param>
/// <param name="Reason">Indicates the reason why these candidates can be eliminated.</param>
public readonly record struct ExocetElimination(Conclusion[] Eliminations, ExocetEliminatedReason Reason) :
	IEquatable<ExocetElimination>,
	IEqualityOperators<ExocetElimination, ExocetElimination>
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(scoped in ExocetElimination other)
		=> Enumerable.SequenceEqual(Eliminations, other.Eliminations) && Reason == other.Reason;

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var hashCode = new HashCode();
		foreach (var conclusion in Eliminations)
		{
			hashCode.Add(conclusion);
		}

		hashCode.Add(Reason);
		return hashCode.ToHashCode();
	}

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		string header = R[$"Exocet{Reason}EliminationName"]!;
		string snippet = R["ExocetElimination"]!;
		string elim = new ConclusionCollection(Eliminations).ToString();
		return $"* {header}{snippet}{elim}";
	}

	/// <summary>
	/// Gets the enumerator of the current instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public OneDimensionalArrayEnumerator<Conclusion> GetEnumerator() => Eliminations.EnumerateImmutable();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool IEquatable<ExocetElimination>.Equals(ExocetElimination other) => Equals(other);
}
