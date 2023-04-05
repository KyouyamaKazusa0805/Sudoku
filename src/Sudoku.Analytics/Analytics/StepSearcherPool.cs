namespace Sudoku.Analytics;

/// <summary>
/// Represents a provider type that can find built-in <see cref="StepSearcher"/> instances.
/// </summary>
/// <seealso cref="StepSearcher"/>
public abstract class StepSearcherPool
{
	/// <summary>
	/// Indicates an array of all built-in <see cref="StepSearcher"/>s that are defined in this assembly.
	/// </summary>
	/// <param name="separated">
	/// Indicates whether we should treat the type as separated one, which means it will be stored multiple times
	/// if it has been marked <see cref="SeparatedAttribute"/>.
	/// </param>
	/// <returns>An array of <see cref="StepSearcher"/> instances found.</returns>
	/// <seealso cref="SeparatedAttribute"/>
	public static StepSearcher[] Default(bool separated = true)
	{
		var result = new SortedList<int, StepSearcher>();
		foreach (var type in typeof(StepSearcherPool).Assembly.GetDerivedTypes(typeof(StepSearcher)))
		{
			if (!type.IsDefined(typeof(StepSearcherAttribute)))
			{
				continue;
			}

			if (!type.HasParameterlessConstructor())
			{
				continue;
			}

			foreach (var stepSearcher in GetStepSearchers(type, separated))
			{
				result.Add(stepSearcher.Priority << 4 | stepSearcher.SeparatedPriority, stepSearcher);
			}
		}

		return result.Values.ToArray();
	}

	/// <summary>
	/// The internal method to get all <see cref="StepSearcher"/> instances derived from <paramref name="type"/> defined in this assembly.
	/// </summary>
	/// <param name="type">The type of the step searcher.</param>
	/// <param name="separated">
	/// <inheritdoc cref="Default(bool)" path="/param[@name='separated']"/>
	/// </param>
	/// <returns><inheritdoc cref="Default(bool)" path="/returns"/></returns>
	/// <seealso cref="SeparatedAttribute"/>
	private static StepSearcher[] GetStepSearchers(Type type, bool separated)
	{
		// Check whether the step searcher is marked 'SeparatedAttribute'.
		switch (type.GetCustomAttributes<SeparatedAttribute>().ToArray())
		{
			case { Length: var length and not 0 } separatedAttributes when separated:
			{
				var i = 0;
				var stepSearcherArray = new StepSearcher[length];

				// If the step searcher is marked 'SeparatedAttribute', we should sort them via priority at first.
				Array.Sort(separatedAttributes, static (a, b) => a.Priority.CompareTo(b.Priority));

				// Then assign property values via reflection.
				foreach (var separatedAttribute in separatedAttributes)
				{
					var instance = (StepSearcher)Activator.CreateInstance(type)!;
					foreach (var (name, value) in separatedAttribute.PropertyNamesAndValues.EnumerateAsPair<object, string, object>())
					{
						if (type.GetProperty(name) is { CanRead: true, CanWrite: true } propertyInfo)
						{
							// Assigns the property with attribute-configured value.
							// Please note that C# 9 feature "init-only" property is a compiler feature, rather than runtime one,
							// which means we can use flection to set value to that property
							// no matter what the setter keyword is 'get' or 'init'.
							propertyInfo.SetValue(instance, value);
						}
					}

					// Sets the separated priority value.
					// We should use reflection to set value because keyword used of the property is 'init', rather than 'set'.
					type.GetProperty(nameof(instance.SeparatedPriority))!
						.GetSetMethod(true)!
						.Invoke(instance, new object[] { separatedAttribute.Priority });

					stepSearcherArray[i++] = instance;
				}

				return stepSearcherArray;
			}
			default:
			{
				return new[] { (StepSearcher)Activator.CreateInstance(type)! };
			}
		}
	}
}
