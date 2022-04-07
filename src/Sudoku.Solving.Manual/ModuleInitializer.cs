namespace Sudoku.Solving;

/// <include
///     file='../../global-doc-comments.xml'
///     path='g/csharp9/feature[@name="module-initializer"]/target[@name="type"]' />
internal static class ModuleInitializer
{
	/// <include
	///     file='../../global-doc-comments.xml'
	///     path='g/csharp9/feature[@name="module-initializer"]/target[@name="method"]' />
	[ModuleInitializer]
	public static void Initialize()
	{
		var listOfStepSearchers = new List<IStepSearcher>();
		foreach (var type in typeof(ModuleInitializer).Assembly.GetTypes())
		{
			// The step searcher must be applied the attribute 'StepSearcherAttribute'.
			if (type.IsDefined(typeof(StepSearcherAttribute))
				|| type.GetCustomAttribute<StepSearcherOptionsAttribute>() is not { IsDeprecated: false })
			{
				continue;
			}

			// The step searcher must implement the interface 'IStepSearcher'.
			if (!type.IsAssignableFrom(typeof(IStepSearcher)))
			{
				continue;
			}

			// The step searcher must contain a parameterless instance constructor.
			if (type.GetConstructors().All(static c => c.GetParameters().Length != 0))
			{
				continue;
			}

			// Now checks whether the step searcher can be seperated into multiple instances.
			// If so, we should create instances one by one, and assign the properties with values
			// using the values inside the type 'SeparatedStepSearcherAttribute'.
			switch (type.GetCustomAttributes<SeparatedStepSearcherAttribute>().ToArray())
			{
				case { Length: not 0 } optionalAssignments:
				{
					// Sort the attribute instances via the priority.
					Array.Sort(optionalAssignments, new SeparatedStepSearcherAttributeComparer());

					// Iterate on each attribute instances.
					foreach (var attributeInstance in optionalAssignments)
					{
						// Creates an instance.
						var instance = (IStepSearcher)Activator.CreateInstance(type)!;

						// Checks the inner values, in order to be used later.
						object[] propertyNamesAndValues = attributeInstance.PropertyNamesAndValues;
						switch (propertyNamesAndValues.Length)
						{
							case 0:
							{
								throw new InvalidOperationException("The array is empty.");
							}
							case var length when (length & 1) != 0:
							{
								throw new InvalidOperationException("The property value is invalid.");
							}
							case var length:
							{
								for (int i = 0; i < length; i += 2)
								{
									string propertyName = (string)propertyNamesAndValues[i];
									object propertyValue = propertyNamesAndValues[i + 1];

									(
										type.GetProperty(propertyName) switch
										{
											null => throw new InvalidOperationException("Such property name cannot be found."),
											{ CanWrite: false } => throw new InvalidOperationException("The property is read-only and cannot be assigned."),
											var p => p
										}
									).SetValue(instance, propertyValue);
								}

								break;
							}
						}

						listOfStepSearchers.Add(instance);
					}

					break;
				}
				default:
				{
					listOfStepSearchers.Add((IStepSearcher)Activator.CreateInstance(type)!);

					break;
				}
			}
		}

		// Assign the result.
		StepSearcherPool.Collection = listOfStepSearchers.ToArray();
	}
}
