namespace Sudoku.Solving.Collections;

/// <summary>
/// Defines an elimination that is created by searchers
/// <see cref="IJuniorExocetStepSearcher"/> and <see cref="ISeniorExocetStepSearcher"/>.
/// </summary>
/// <param name="Eliminations">Indicates the eliminations.</param>
/// <param name="Reason">Indicates the reason why these candidates can be eliminated.</param>
/// <seealso cref="IJuniorExocetStepSearcher"/>
/// <seealso cref="ISeniorExocetStepSearcher"/>
[AutoDeconstruct(nameof(Eliminations), nameof(Reason))]
[AutoGetHashCode(nameof(Eliminations), nameof(Reason))]
[AutoEquality(nameof(Eliminations), nameof(Reason))]
[AutoGetEnumerator("@", MemberConversion = $"@.{nameof(AsSpan)}().*", ReturnType = typeof(ReadOnlySpan<Conclusion>.Enumerator))]
public readonly partial record struct ExocetElimination(in Candidates Eliminations, ExocetEliminatedReason Reason)
{
	/// <summary>
	/// Indicates how many eliminations the instance contains.
	/// </summary>
	public int Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Eliminations.Count;
	}

	/// <summary>
	/// Indicates the header of the reason.
	/// </summary>
	private string Header => ResourceDocumentManager.Shared[$"exocet{Reason}EliminationName"];


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		string snippet = ResourceDocumentManager.Shared["exocetElimination"];
		string elim = new ConclusionCollection(AsSpan()).ToString();
		return $"* {Header}{snippet}{elim}";
	}

	/// <summary>
	/// Converts all elements to <see cref="Conclusion"/>.
	/// </summary>
	/// <returns>The <see cref="ReadOnlySpan{T}"/> of type <see cref="Conclusion"/>.</returns>
	/// <seealso cref="ReadOnlySpan{T}"/>
	/// <seealso cref="Conclusion"/>
	public ReadOnlySpan<Conclusion> AsSpan()
	{
		var result = new Conclusion[Eliminations.Count];
		for (int i = 0, count = Eliminations.Count; i < count; i++)
		{
			result[i] = new(ConclusionType.Elimination, Eliminations[i]);
		}

		return result;
	}


	/// <summary>
	/// To merge two different instances, and return the merged result.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>
	/// The merged result. The result will contain all eliminations from two instances, and
	/// the reason should be same.
	/// </returns>
	/// <exception cref="ArgumentException">
	/// Throws when two instances contains different eliminated reason.
	/// </exception>
	public static unsafe ExocetElimination operator |(in ExocetElimination left, in ExocetElimination right)
	{
		var (le, lr) = left;
		var (re, rr) = right;

		if (lr != rr)
		{
			throw new ArgumentException("Two arguments should contains same eliminated reason.");
		}

		int totalCount = le.Count + re.Count;
		int* merged = stackalloc int[totalCount];
		for (int i = 0, count = le.Count; i < count; i++)
		{
			merged[i] = left.Eliminations[i];
		}
		for (int i = 0, count = re.Count; i < count; i++)
		{
			merged[i + le.Count] = re[i];
		}

		return new(new(merged, totalCount), lr);
	}
}
