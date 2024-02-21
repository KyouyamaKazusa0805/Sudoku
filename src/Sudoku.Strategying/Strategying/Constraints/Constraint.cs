namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a rule that checks whether a <see cref="Grid"/> is passed the constraint.
/// </summary>
/// <seealso cref="Grid"/>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$typeid", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
[JsonDerivedType(typeof(CountConstraint), nameof(CountConstraint))]
[JsonDerivedType(typeof(AnalyzerTechniqueConstraint), nameof(AnalyzerTechniqueConstraint))]
[Equals(OtherModifiers = "sealed")]
[EqualityOperators]
public abstract partial class Constraint : IEquatable<Constraint>, IEqualityOperators<Constraint, Constraint, bool>
{
	/// <summary>
	/// Indicates the properties that will be used in constraint checking.
	/// </summary>
	public abstract ConstraintCheckingProperty ConstraintCheckingProperties { get; }

	/// <summary>
	/// Indicates the validation result.
	/// </summary>
	protected internal abstract ValidationResult ValidationResult { get; }


	/// <summary>
	/// Determine whether the specified grid is passed the constraint.
	/// </summary>
	/// <param name="context">Indicates the context used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Check(scoped ConstraintCheckingContext context) => ValidationResult.Success && CheckCore(context);

	/// <inheritdoc/>
	public abstract bool Equals([NotNullWhen(true)] Constraint? other);

	/// <inheritdoc/>
	public abstract override int GetHashCode();

	/// <inheritdoc/>
	public abstract override string ToString();

	/// <inheritdoc cref="Check"/>
	/// <remarks><i>
	/// This method only handles for the core rule of the type, which means we should suppose the values are valid.
	/// </i></remarks>
	protected internal abstract bool CheckCore(scoped ConstraintCheckingContext context);
}
