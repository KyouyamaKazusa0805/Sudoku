namespace Sudoku.Behaviors;

/// <summary>
/// Represents a difference result.
/// </summary>
[IntroducedSince(3, 4)]
[TypeImpl(
	TypeImplFlags.AllObjectMethods | TypeImplFlags.EqualityOperators,
	ToStringBehavior = ToStringBehavior.MakeAbstract,
	GetHashCodeBehavior = GetHashCodeBehavior.MakeAbstract,
	OtherModifiersOnEquals = "sealed")]
[JsonPolymorphic]
[JsonDerivedType(typeof(NothingChangedDiffResult), (int)DiffType.NothingChanged)]
[JsonDerivedType(typeof(ResetDiffResult), (int)DiffType.Reset)]
[JsonDerivedType(typeof(AddGivenDiffResult), (int)DiffType.AddGiven)]
[JsonDerivedType(typeof(AddModifiableDiffResult), (int)DiffType.AddModifiable)]
[JsonDerivedType(typeof(AddCandidateDiffResult), (int)DiffType.AddCandidate)]
[JsonDerivedType(typeof(RemoveGivenDiffResult), (int)DiffType.RemoveGiven)]
[JsonDerivedType(typeof(RemoveModifiableDiffResult), (int)DiffType.RemoveModifiable)]
[JsonDerivedType(typeof(RemoveCandidateDiffResult), (int)DiffType.RemoveCandidate)]
public abstract partial class DiffResult :
	ICloneable,
	IEquatable<DiffResult>,
	IEqualityOperators<DiffResult, DiffResult, bool>,
	IFormattable
{
	/// <summary>
	/// Indicates the notation prefix.
	/// </summary>
	public virtual string NotationPrefix => Notation[0].ToString();

	/// <summary>
	/// Indicates the simplified notation of the current difference result.
	/// </summary>
	public abstract string Notation { get; }

	/// <summary>
	/// Indicates the type of the difference.
	/// </summary>
	public abstract DiffType Type { get; }

	/// <summary>
	/// Indicates the target type.
	/// </summary>
	protected Type EqualityContract => GetType();


	/// <inheritdoc/>
	public abstract bool Equals([NotNullWhen(true)] DiffResult? other);

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	/// <exception cref="FormatException">Throws when the format argument is invalid.</exception>
	public string ToString(string? format)
		=> format switch
		{
			null => ToString(),
			"N" or "n" => Notation,
			"T" or "t" => Type.ToString(),
			_ => throw new FormatException()
		};

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	public virtual string ToString(IFormatProvider? formatProvider) => ToString();

	/// <inheritdoc/>
	public string ToString(string? format, IFormatProvider? formatProvider) => ToString(format);

	/// <inheritdoc cref="ICloneable.Clone"/>
	public abstract DiffResult Clone();

	/// <inheritdoc/>
	object ICloneable.Clone() => Clone();
}
