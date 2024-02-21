namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a constraint that checks for techniques, and their appeared counts.
/// </summary>
[ToString]
public sealed partial class CollectorTechniqueConstraint : Constraint
{
	/// <inheritdoc/>
	public override ConstraintCheckingProperty CheckingProperties => ConstraintCheckingProperty.CollectorResult;

	/// <summary>
	/// Indicates the universal quantifier.
	/// </summary>
	[StringMember]
	public required UniversalQuantifier UniversalQuantifier { get; set; }

	/// <summary>
	/// Indicates the dictionary that stores the appearing cases on each technique.
	/// </summary>
	/// <remarks>
	/// The return type of this property is <see cref="FrozenDictionary{TKey, TValue}"/>. You can create an instance
	/// by using extension method <c>ToFrozenDictionary</c> in type <see cref="FrozenDictionary"/>.
	/// </remarks>
	/// <seealso cref="FrozenDictionary"/>
	public required FrozenDictionary<Technique, int> TechniqueAppearing { get; set; }

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
		=> other is CollectorTechniqueConstraint comparer
		&& DictionaryEquals(TechniqueAppearing, comparer.TechniqueAppearing, UniversalQuantifier.All);

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var hashCode = new HashCode();
		hashCode.Add(UniversalQuantifier);

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
		if (!context.RequiresCollector)
		{
			return false;
		}

		var dic = new Dictionary<Technique, int>();
		foreach (var stepArray in context.CollectorResult)
		{
			foreach (var step in stepArray)
			{
				if (!dic.TryAdd(step.Code, 1))
				{
					dic[step.Code]++;
				}
			}
		}

		return DictionaryGreaterThanOrEquals(dic, TechniqueAppearing, UniversalQuantifier);
	}

	/// <inheritdoc/>
	protected internal override ValidationResult ValidateCore()
	{
		foreach (var element in TechniqueAppearing.Values)
		{
			if (element < 0)
			{
				return ValidationResult.Failed(
					nameof(TechniqueAppearing),
					ValidationReason.OutOfRange,
					Severity.Error
				);
			}
		}

		return ValidationResult.Successful;
	}
}
