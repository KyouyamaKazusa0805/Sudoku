namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that measures the number of petals appeared in a <see cref="DeathBlossomStep"/>.
/// </summary>
/// <typeparam name="TStep">The type of the step.</typeparam>
/// <typeparam name="TCollection">The type of itself.</typeparam>
/// <typeparam name="TKey">The type of the distinction key.</typeparam>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="DeathBlossomStep"/>
public abstract class DeathBlossomPetalsCountFactor<TStep, TCollection, TKey>(StepSearcherOptions options) : Factor(options)
	where TStep : DeathBlossomBaseStep, IDeathBlossomCollection<TCollection, TKey>
	where TCollection :
		DeathBlossomBranchCollection<TCollection, TKey>,
		IEquatable<TCollection>,
		IEqualityOperators<TCollection, TCollection, bool>,
		new()
	where TKey : notnull, IAdditiveIdentity<TKey, TKey>, IEquatable<TKey>, IEqualityOperators<TKey, TKey, bool>, new()
{
	/// <inheritdoc/>
	public sealed override string FormulaString => "A002024({0}.Count)";

	/// <inheritdoc/>
	public sealed override string[] ParameterNames => [nameof(IDeathBlossomCollection<TCollection, TKey>.Branches)];

	/// <inheritdoc/>
	public sealed override Type ReflectedStepType => typeof(TStep);

	/// <inheritdoc/>
	public sealed override Func<Step, int?> Formula
		=> static step => step switch { TStep { Branches.Count: var count } => A002024(count), _ => null };
}
