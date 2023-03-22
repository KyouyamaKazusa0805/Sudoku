namespace Sudoku.Analytics;

/// <summary>
/// Represents a provider type that can find built-in <see cref="StepSearcher"/> instances.
/// </summary>
/// <seealso cref="StepSearcher"/>
public abstract class StepSearcherPool
{
	/// <summary>
	/// Indicates an array of built-in <see cref="StepSearcher"/>s.
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

				if (!type.HasParameterlessConstructor())
				{
					continue;
				}

				// Check whether the step searcher is marked 'SeparatedAttribute'.
				switch (type.GetCustomAttributes<SeparatedAttribute>().ToArray())
				{
					case { Length: var length and not 0 } separatedAttributes:
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

							stepSearcherArray[i++] = instance;
						}

						result.AddRange(stepSearcherArray);

						break;
					}
					default:
					{
						result.Add((StepSearcher)Activator.CreateInstance(type)!);
						break;
					}
				}
			}

			result.Sort(static (a, b) => a.Priority.CompareTo(b.Priority));
			return result.ToArray();
		}
	}
}
