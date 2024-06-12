namespace Sudoku.Analytics;

/// <summary>
/// Represents a provider type that can find built-in <see cref="StepSearcher"/> instances.
/// </summary>
/// <seealso cref="StepSearcher"/>
public static class StepSearcherPool
{
	/// <summary>
	/// Indicates the current assembly.
	/// </summary>
	private static readonly Assembly ThisAssembly = typeof(StepSearcherPool).Assembly;


	/// <summary>
	/// Try to fetch a list of built-in step searchers.
	/// </summary>
	public static ReadOnlyMemory<StepSearcher> BuiltInStepSearchers
	{
		get
		{
			var result = new SortedList<int, StepSearcher>();
			foreach (var type in ThisAssembly.GetDerivedTypes<StepSearcher>())
			{
				if (!type.IsDefined<StepSearcherAttribute>())
				{
					continue;
				}

				if (!type.HasParameterlessConstructor())
				{
					continue;
				}

				var instance = GetStepSearchers(type);
				result.Add(instance.Priority, instance);
			}
			return result.Values.ToArray().AsMemory();
		}
	}


	/// <summary>
	/// <inheritdoc cref="GetStepSearchers(Type)" path="/summary"/>
	/// </summary>
	/// <param name="typeName">The raw type name. Please note that the string text shouldn't contain its containing namespace.</param>
	/// <returns><inheritdoc cref="GetStepSearchers(Type)" path="/returns"/></returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the corresponding <see cref="Type"/> reflection result is not found.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static StepSearcher GetStepSearcher(string typeName)
		=> GetStepSearchers(ThisAssembly.GetType($"Sudoku.Analytics.StepSearchers.{typeName}")!);

	/// <summary>
	/// The internal method to get all <see cref="StepSearcher"/> instances derived from <paramref name="type"/> defined in this assembly.
	/// </summary>
	/// <param name="type">The type of the step searcher.</param>
	/// <returns>An array of <see cref="StepSearcher"/> instances found.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static StepSearcher GetStepSearchers(Type type) => (StepSearcher)Activator.CreateInstance(type)!;
}
