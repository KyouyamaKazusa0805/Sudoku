namespace Sudoku.Analytics;

public partial class __CollectorFactoryMethods_Add
{
	/// <summary>
	/// Appends an element into the property <see cref="Collector.Setters"/>.
	/// </summary>
	/// <param name="instance">The instance to be updated.</param>
	/// <param name="setter">The value to be added.</param>
	/// <returns>The value same as <see cref="Collector"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Collector AddStepSearcherSetter<TStepSearcher>(this Collector instance, Action<TStepSearcher> setter)
		where TStepSearcher : StepSearcher
	{
		instance.Setters.Add(
			s =>
			{
				if (s is TStepSearcher target)
				{
					setter(target);
				}
			}
		);
		return instance;
	}
}
