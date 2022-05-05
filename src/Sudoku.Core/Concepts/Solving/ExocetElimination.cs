namespace Sudoku.Concepts.Solving;

/// <summary>
/// Represents a data structure that describes the eliminations that are created and proved
/// by the exocet technique.
/// </summary>
/// <param name="Eliminations">Indicates the eliminations.</param>
/// <param name="Reason">Indicates the reason why these candidates can be eliminated.</param>
[AutoOverridesGetHashCode(nameof(Eliminations), nameof(Reason))]
[AutoOverridesEquals(nameof(Eliminations), nameof(Reason), EmitsInKeyword = true, UseExplicitImplementation = true)]
public readonly partial record struct ExocetElimination(in Candidates Eliminations, ExocetEliminatedReason Reason) :
	IEquatable<ExocetElimination>,
	IEqualityOperators<ExocetElimination, ExocetElimination>
{
	/// <summary>
	/// Indicates how many eliminations the instance contains.
	/// </summary>
	public int Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Eliminations.Count;
	}


	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
#if true
		// Use resource to get the result.
		string header = R[$"Exocet{Reason}EliminationName"]!;
		string snippet = R["ExocetElimination"]!;
		string elim = new ConclusionCollection(ToArray()).ToString();
		return $"* {header}{snippet}{elim}";
#else
		// Use attribute to get the result.
		string header = typeof(ExocetEliminatedReason)
			.GetField(Reason.ToString())!
			.GetCustomAttribute<EnumFieldNameAttribute>()!
			.Name;

		return $"* {header}eliminations: {new ConclusionCollection(ToArray()).ToString()}";
#endif
	}

	/// <summary>
	/// Gets the enumerator of the current instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public OneDimensionalArrayEnumerator<Conclusion> GetEnumerator() => ToArray().EnumerateImmutable();

	/// <summary>
	/// Converts all elements to <see cref="Conclusion"/>.
	/// </summary>
	/// <returns>The <see cref="ReadOnlySpan{T}"/> of type <see cref="Conclusion"/>.</returns>
	/// <seealso cref="ReadOnlySpan{T}"/>
	/// <seealso cref="Conclusion"/>
	public Conclusion[] ToArray()
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
		_ = left is { Eliminations: var le, Reason: var lr };
		_ = right is { Eliminations: var re, Reason: var rr };
		Argument.ThrowIfFalse(lr == rr, "Two arguments should contains same eliminated reason.");

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
