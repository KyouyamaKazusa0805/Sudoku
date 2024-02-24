namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a rule that checks whether a grid or its relied analysis information is passed the constraint.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$typeid", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
[JsonDerivedType(typeof(AnalyzerTechniqueConstraint), nameof(AnalyzerTechniqueConstraint))]
[JsonDerivedType(typeof(AnalyzerTechniqueCountConstraint), nameof(AnalyzerTechniqueCountConstraint))]
[JsonDerivedType(typeof(AnalyzerTechniqueGroupConstraint), nameof(AnalyzerTechniqueGroupConstraint))]
[JsonDerivedType(typeof(AnalyzerTechniqueNameConstraint), nameof(AnalyzerTechniqueNameConstraint))]
[JsonDerivedType(typeof(BottleneckConstraint), nameof(BottleneckConstraint))]
[JsonDerivedType(typeof(ConclusionConstraint), nameof(ConclusionConstraint))]
[JsonDerivedType(typeof(CountConstraint), nameof(CountConstraint))]
[JsonDerivedType(typeof(DiamondConstraint), nameof(DiamondConstraint))]
[JsonDerivedType(typeof(DifficultyLevelConstraint), nameof(DifficultyLevelConstraint))]
[JsonDerivedType(typeof(IttoryuConstraint), nameof(IttoryuConstraint))]
[JsonDerivedType(typeof(IttoryuLengthConstraint), nameof(IttoryuLengthConstraint))]
[JsonDerivedType(typeof(MinimalConstraint), nameof(MinimalConstraint))]
[JsonDerivedType(typeof(PearlConstraint), nameof(PearlConstraint))]
[JsonDerivedType(typeof(SymmetryConstraint), nameof(SymmetryConstraint))]
[Equals(OtherModifiers = "sealed")]
[GetHashCode(GetHashCodeBehavior.MakeAbstract)]
[ToString(ToStringBehavior.MakeAbstract)]
[EqualityOperators]
public abstract partial class Constraint : IEquatable<Constraint>, IEqualityOperators<Constraint, Constraint, bool>
{
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
	/// By default, the method always return <see cref="ConflictionResult.Successful"/>.
	/// </summary>
	/// <param name="other">The constraint to be checked.</param>
	/// <returns>A <see cref="bool"/> indicating whether the current constraint will conflict with the specified one.</returns>
	/// <seealso cref="ConflictionResult.Successful"/>
	public virtual ConflictionResult VerifyConfliction(Constraint other) => ConflictionResult.Successful;

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


	/// <summary>
	/// Compares instances <typeparamref name="T1"/> and <typeparamref name="T2"/> with inner values.
	/// </summary>
	/// <typeparam name="T1">The type of the first dictionary.</typeparam>
	/// <typeparam name="T2">The type of the second dictionary.</typeparam>
	/// <param name="left">The first element to be compared.</param>
	/// <param name="right">The second element to be compared.</param>
	/// <param name="universalQuantifier">The universal quantifier.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	private protected static bool DictionaryEquals<T1, T2>(T1 left, T2 right, UniversalQuantifier universalQuantifier)
		where T1 : IDictionary<Technique, int>
		where T2 : IDictionary<Technique, int>
	{
		if (left.Count != right.Count)
		{
			return false;
		}

		foreach (var key in left.Keys)
		{
			if (universalQuantifier == UniversalQuantifier.All)
			{
				if (!right.TryGetValue(key, out var v) || v != left[key])
				{
					return false;
				}
			}
			else
			{
				if (right.TryGetValue(key, out var v) && v == left[key])
				{
					return true;
				}
			}
		}

		return universalQuantifier == UniversalQuantifier.All;
	}

	/// <summary>
	/// Compares instances <typeparamref name="T1"/> and <typeparamref name="T2"/> with inner values.
	/// </summary>
	/// <typeparam name="T1">The type of the first dictionary.</typeparam>
	/// <typeparam name="T2">The type of the second dictionary.</typeparam>
	/// <param name="left">The first element to be compared.</param>
	/// <param name="right">The second element to be compared.</param>
	/// <param name="universalQuantifier">The universal quantifier.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	private protected static bool DictionaryGreaterThanOrEquals<T1, T2>(T1 left, T2 right, UniversalQuantifier universalQuantifier)
		where T1 : IDictionary<Technique, int>
		where T2 : IDictionary<Technique, int>
	{
		if (left.Count < right.Count)
		{
			return false;
		}

		foreach (var key in left.Keys)
		{
			if (universalQuantifier == UniversalQuantifier.All)
			{
				if ((right.TryGetValue(key, out var v) ? v : 0) < left[key])
				{
					return false;
				}
			}
			else
			{
				if ((right.TryGetValue(key, out var v) ? v : 0) >= left[key])
				{
					return true;
				}
			}
		}

		return universalQuantifier == UniversalQuantifier.All;
	}
}
