namespace Sudoku.Strategying;

/// <summary>
/// Represents a rule that checks whether a grid or its relied analysis information is passed the constraint.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$typeid", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
[JsonDerivedType(typeof(ConclusionConstraint), nameof(ConclusionConstraint))]
[JsonDerivedType(typeof(CountBetweenConstraint), nameof(CountBetweenConstraint))]
[JsonDerivedType(typeof(DiamondConstraint), nameof(DiamondConstraint))]
[JsonDerivedType(typeof(DifficultyLevelConstraint), nameof(DifficultyLevelConstraint))]
[JsonDerivedType(typeof(IttoryuConstraint), nameof(IttoryuConstraint))]
[JsonDerivedType(typeof(IttoryuLengthConstraint), nameof(IttoryuLengthConstraint))]
[JsonDerivedType(typeof(MinimalConstraint), nameof(MinimalConstraint))]
[JsonDerivedType(typeof(PearlConstraint), nameof(PearlConstraint))]
[JsonDerivedType(typeof(SymmetryConstraint), nameof(SymmetryConstraint))]
[JsonDerivedType(typeof(TechniqueConstraint), nameof(TechniqueConstraint))]
[JsonDerivedType(typeof(TechniqueCountConstraint), nameof(TechniqueCountConstraint))]
[Equals(OtherModifiers = "sealed")]
[GetHashCode(GetHashCodeBehavior.MakeAbstract)]
[ToString(ToStringBehavior.MakeAbstract)]
[EqualityOperators]
public abstract partial class Constraint :
	ICultureFormattable,
	IEquatable<Constraint>,
	IEqualityOperators<Constraint, Constraint, bool>
{
	/// <summary>
	/// Indicates whether the constraint can duplicate.
	/// </summary>
	public abstract bool AllowDuplicate { get; }

	/// <summary>
	/// Indicates the checking <see cref="Expression{TDelegate}"/> instance of the constraint.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This property will be reserved, in order to combine multiple constraint checking rules by using custom boolean logic.
	/// For example, a user may want either one of two constraints satisfy rather than checking for both two constraints.
	/// By using this property, use <see cref="Expression.OrElse(Expression, Expression)"/> to achieve the goal.
	/// </para>
	/// <para>
	/// Due to implement this property and limitation of <see cref="Expression{TDelegate}"/>, we cannot apply the type
	/// <see cref="ConstraintCheckingContext"/> as a <see langword="ref struct"/> because <see cref="Expression{TDelegate}"/>
	/// doesn't support for <see langword="ref struct"/> types.
	/// </para>
	/// </remarks>
	/// <seealso cref="Expression{TDelegate}"/>
	/// <seealso cref="Expression.OrElse(Expression, Expression)"/>
	/// <seealso cref="ConstraintCheckingContext"/>
	public Expression<Func<Constraint, ConstraintCheckingContext, bool>> CheckingQueryExpression => static (constraint, context) => constraint.Check(context);


	/// <summary>
	/// Determine whether the specified grid is passed the constraint.
	/// </summary>
	/// <param name="context">Indicates the context used.</param>
	public abstract bool Check(ConstraintCheckingContext context);

	/// <inheritdoc/>
	public abstract bool Equals([NotNullWhen(true)] Constraint? other);

	/// <inheritdoc/>
	public abstract string ToString(CultureInfo? culture = null);
}
