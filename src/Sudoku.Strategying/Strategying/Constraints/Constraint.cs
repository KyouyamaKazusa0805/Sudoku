namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a rule that checks whether a grid or its relied analysis information is passed the constraint.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$typeid", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
[JsonDerivedType(typeof(AnalyzerTechniqueConstraint), nameof(AnalyzerTechniqueConstraint))]
[JsonDerivedType(typeof(AnalyzerTechniqueCountConstraint), nameof(AnalyzerTechniqueCountConstraint))]
[JsonDerivedType(typeof(AnalyzerTechniqueGroupConstraint), nameof(AnalyzerTechniqueGroupConstraint))]
[JsonDerivedType(typeof(AnalyzerTechniqueNameConstraint), nameof(AnalyzerTechniqueNameConstraint))]
[JsonDerivedType(typeof(CollectorTechniqueConstraint), nameof(CollectorTechniqueConstraint))]
[JsonDerivedType(typeof(CollectorTechniqueCountConstraint), nameof(CollectorTechniqueCountConstraint))]
[JsonDerivedType(typeof(CollectorTechniqueGroupConstraint), nameof(CollectorTechniqueGroupConstraint))]
[JsonDerivedType(typeof(CollectorTechniqueNameConstraint), nameof(CollectorTechniqueNameConstraint))]
[JsonDerivedType(typeof(ConclusionConstraint), nameof(ConclusionConstraint))]
[JsonDerivedType(typeof(CountConstraint), nameof(CountConstraint))]
[JsonDerivedType(typeof(DiamondConstraint), nameof(DiamondConstraint))]
[JsonDerivedType(typeof(PearlConstraint), nameof(PearlConstraint))]
[Equals(OtherModifiers = "sealed")]
[GetHashCode(GetHashCodeBehavior.MakeAbstract)]
[ToString(ToStringBehavior.MakeAbstract)]
[EqualityOperators]
public abstract partial class Constraint : IEquatable<Constraint>, IEqualityOperators<Constraint, Constraint, bool>
{
	/// <summary>
	/// Indicates the properties that will be used in constraint checking.
	/// </summary>
	public abstract ConstraintCheckingProperty CheckingProperties { get; }


	/// <summary>
	/// Determine whether the specified grid is passed the constraint.
	/// </summary>
	/// <param name="context">Indicates the context used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Check(scoped ConstraintCheckingContext context) => ValidateCore().IsSuccess && CheckCore(context);

	/// <inheritdoc/>
	public abstract bool Equals([NotNullWhen(true)] Constraint? other);

	/// <summary>
	/// Determine whether the current constraint will raise a confliction with the specified constraint.
	/// </summary>
	/// <param name="other">The constraint to be checked.</param>
	/// <returns>A <see cref="bool"/> indicating whether the current constraint will conflict with the specified one.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ConflictionResult VerifyConfliction(Constraint other) => VerifyConflictionCore(other);

	/// <inheritdoc cref="Check"/>
	/// <remarks><i>
	/// This method only handles for the core rule of the type, which means we should suppose the values are valid.
	/// </i></remarks>
	protected internal abstract bool CheckCore(scoped ConstraintCheckingContext context);

	/// <summary>
	/// Verifies the validity of properties set.
	/// </summary>
	/// <returns>A <see cref="ValidationResult"/> instance describing the final result on validation.</returns>
	protected internal abstract ValidationResult ValidateCore();

	/// <inheritdoc cref="VerifyConfliction(Constraint)"/>
	/// <remarks><inheritdoc cref="CheckCore(ConstraintCheckingContext)" path="/remarks"/></remarks>
	protected internal virtual ConflictionResult VerifyConflictionCore(Constraint other) => ConflictionResult.Successful;
}
