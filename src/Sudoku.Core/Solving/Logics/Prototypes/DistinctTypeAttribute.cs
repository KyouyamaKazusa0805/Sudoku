namespace Sudoku.Solving.Logics.Prototypes;

/// <summary>
/// Defines an internal type that controls the customized equality comparing operation
/// of type <typeparamref name="TEqualityComparer"/> on elements of type <typeparamref name="TStep"/>.
/// </summary>
/// <typeparam name="TStep">The type of the step.</typeparam>
/// <typeparam name="TEqualityComparer">The equality comparer to filter duplicated items.</typeparam>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal sealed class DistinctTypeAttribute<TStep, TEqualityComparer> : Attribute
	where TStep : Step, IDistinctableStep<TStep>
	where TEqualityComparer : class, IEqualityComparer<TStep>, new()
{
}
