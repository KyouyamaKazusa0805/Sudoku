namespace Sudoku.Generating.Filtering;

/// <summary>
/// Represents a rule that checks whether a grid or its relied analysis information is passed the constraint.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$typeid", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
[JsonDerivedType(typeof(BottleneckTechniqueConstraint), nameof(BottleneckTechniqueConstraint))]
[JsonDerivedType(typeof(ConclusionConstraint), nameof(ConclusionConstraint))]
[JsonDerivedType(typeof(CountBetweenConstraint), nameof(CountBetweenConstraint))]
[JsonDerivedType(typeof(DiamondConstraint), nameof(DiamondConstraint))]
[JsonDerivedType(typeof(DifficultyLevelConstraint), nameof(DifficultyLevelConstraint))]
[JsonDerivedType(typeof(EliminationCountConstraint), nameof(EliminationCountConstraint))]
[JsonDerivedType(typeof(IttoryuConstraint), nameof(IttoryuConstraint))]
[JsonDerivedType(typeof(LastingConstraint), nameof(LastingConstraint))]
[JsonDerivedType(typeof(MinimalConstraint), nameof(MinimalConstraint))]
[JsonDerivedType(typeof(PearlConstraint), nameof(PearlConstraint))]
[JsonDerivedType(typeof(PrimarySingleConstraint), nameof(PrimarySingleConstraint))]
[JsonDerivedType(typeof(SymmetryConstraint), nameof(SymmetryConstraint))]
[JsonDerivedType(typeof(TechniqueConstraint), nameof(TechniqueConstraint))]
[JsonDerivedType(typeof(TechniqueCountConstraint), nameof(TechniqueCountConstraint))]
[JsonDerivedType(typeof(TechniqueSetConstraint), nameof(TechniqueSetConstraint))]
[TypeImpl(
	TypeImplFlag.AllObjectMethods | TypeImplFlag.EqualityOperators,
	OtherModifiersOnEquals = "sealed",
	GetHashCodeBehavior = GetHashCodeBehavior.MakeAbstract,
	ToStringBehavior = ToStringBehavior.MakeAbstract)]
public abstract partial class Constraint : IEquatable<Constraint>, IEqualityOperators<Constraint, Constraint, bool>, IFormattable
{
	/// <summary>
	/// Indicates whether the constraint should be negated.
	/// </summary>
	public bool IsNegated { get; set; }

	/// <summary>
	/// Indicates the description to the constraint.
	/// </summary>
	public string Description => SR.Get($"ConstraintDescription_{GetType().Name}");


	/// <summary>
	/// Determine whether the specified grid is passed the constraint.
	/// </summary>
	/// <param name="context">Indicates the context used.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Check(ConstraintCheckingContext context) => CheckCore(context) is var result && (IsNegated ? !result : result);

	/// <inheritdoc/>
	public abstract bool Equals([NotNullWhen(true)] Constraint? other);

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	public abstract string ToString(IFormatProvider? formatProvider);

	/// <summary>
	/// Creates a new instance that contains same data with the current instance.
	/// </summary>
	/// <returns>A new instance that contains same data with the current instance.</returns>
	public abstract Constraint Clone();

	/// <summary>
	/// Returns a <see cref="ConstraintOptionsAttribute"/> instance that represents the metadata of the constraint configured.
	/// </summary>
	/// <returns>A <see cref="ConstraintOptionsAttribute"/> instance or <see langword="null"/> if not configured.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ConstraintOptionsAttribute? GetMetadata() => GetType().GetCustomAttribute<ConstraintOptionsAttribute>();

	/// <inheritdoc cref="Check(ConstraintCheckingContext)"/>
	protected abstract bool CheckCore(ConstraintCheckingContext context);

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);
}
