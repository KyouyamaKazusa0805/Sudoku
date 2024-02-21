namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a constraint that checks for techniques, and their appeared counts.
/// </summary>
[ToString]
public sealed partial class TechniqueCountConstraint : Constraint
{
	/// <inheritdoc/>
	public override ConstraintCheckingProperty ConstraintCheckingProperties => ConstraintCheckingProperty.AnalyzerResult;

	/// <summary>
	/// Indicates the dictionary that stores the appearing cases on each technique.
	/// </summary>
	/// <remarks>
	/// The return type of this property is <see cref="FrozenDictionary{TKey, TValue}"/>. You can create an instance
	/// by using extension method <c>ToFrozenDictionary</c> in type <see cref="FrozenDictionary"/>.
	/// </remarks>
	/// <seealso cref="FrozenDictionary"/>
	public required FrozenDictionary<Technique, int> TechniqueAppearing { get; set; }

	/// <inheritdoc/>
	protected internal override ValidationResult ValidationResult
	{
		get
		{
			foreach (var element in TechniqueAppearing.Values)
			{
				if (element < 0)
				{
					return new FailedValidationResult(
						nameof(TechniqueAppearing),
						ValidationFailedReason.OutOfRange,
						ValidationFailedSeverity.Error
					);
				}
			}

			return new SuccessValidationResult();
		}
	}

	[StringMember]
	private string TechniqueAppearingString
		=> string.Join(
			", ",
			from kvp in TechniqueAppearing
			let technique = kvp.Key
			let count = kvp.Value
			select $"[{technique.GetName()}: {count}]"
		);


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is TechniqueCountConstraint comparer && DictionaryEquals(TechniqueAppearing, comparer.TechniqueAppearing);

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var hashCode = new HashCode();
		foreach (var (technique, count) in TechniqueAppearing)
		{
			hashCode.Add(technique);
			hashCode.Add(count);
		}

		return hashCode.ToHashCode();
	}

	/// <inheritdoc/>
	protected internal override bool CheckCore(ConstraintCheckingContext context)
	{
		if (!context.RequiresAnalyzer)
		{
			return false;
		}

		var dic = new Dictionary<Technique, int>();
		foreach (var step in context.AnalyzerResult)
		{
			if (!dic.TryAdd(step.Code, 1))
			{
				dic[step.Code]++;
			}
		}

		return DictionaryEquals(dic, TechniqueAppearing);
	}


	/// <summary>
	/// Compares instances <typeparamref name="T1"/> and <typeparamref name="T2"/> with inner values.
	/// </summary>
	/// <typeparam name="T1">The type of the first dictionary.</typeparam>
	/// <typeparam name="T2">The type of the second dictionary.</typeparam>
	/// <param name="left">The first element to be compared.</param>
	/// <param name="right">The second element to be compared.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	private static bool DictionaryEquals<T1, T2>(T1 left, T2 right)
		where T1 : IDictionary<Technique, int>
		where T2 : IDictionary<Technique, int>
	{
		if (left.Count != right.Count)
		{
			return false;
		}

		foreach (var key in left.Keys)
		{
			if (!right.TryGetValue(key, out var v) || v != left[key])
			{
				return false;
			}
		}

		return true;
	}
}
