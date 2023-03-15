namespace Sudoku.Analytics;

/// <summary>
/// Represents a provider type that can find built-in <see cref="StepSearcher"/> instances.
/// </summary>
/// <seealso cref="StepSearcher"/>
public abstract class StepSearcherPool
{
	/// <summary>
	/// Indicates an array of built-in <see cref="StepSearcher"/>s
	/// </summary>
	public static StepSearcher[] BuiltIn
	{
		get
		{
			var result = new List<StepSearcher>();
			foreach (var type in typeof(StepSearcherPool).Assembly.GetDerivedTypes(typeof(StepSearcher)))
			{
				if (!type.IsDefined(typeof(StepSearcherAttribute)))
				{
					continue;
				}

				if (type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, Type.EmptyTypes) is null)
				{
					continue;
				}

				result.Add((StepSearcher)Activator.CreateInstance(type)!);
			}

			// TODO: Separated step searchers.

			result.Sort(static (a, b) => a.Priority > b.Priority ? 1 : a.Priority < b.Priority ? -1 : 0);
			return result.ToArray();
		}
	}
}
