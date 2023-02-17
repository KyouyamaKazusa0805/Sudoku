namespace Sudoku.Buffers;

/// <summary>
/// Indicates a pool that stores a <see cref="StepSearcherCollection"/> instance.
/// </summary>
/// <seealso cref="StepSearcherCollection"/>
public static class StepSearcherPool
{
	/// <summary>
	/// The step searchers to find steps to apply to a certain puzzle.
	/// </summary>
	internal static StepSearcherCollection Collection = null!;


	/// <summary>
	/// Gets the default collection of step searchers via reflection.
	/// </summary>
	public static StepSearcherCollection DefaultCollection
	{
		get
		{
			// Initializes for step searchers.
			var listOfStepSearchers = new List<IStepSearcher>();
			foreach (var type in typeof(ModuleInitializer).Assembly.GetTypes())
			{
				// The step searcher must be applied the attribute 'StepSearcherAttribute'.
				if (!type.IsDefined(typeof(StepSearcherAttribute)))
				{
					continue;
				}

				// The step searcher cannot be deprecated.
				if (type.GetCustomAttribute<StepSearcherRunningOptionsAttribute>() is { Options: var options }
					&& options.Flags(StepSearcherRunningOptions.TemporarilyDisabled))
				{
					continue;
				}

				// The step searcher must implement the interface 'IStepSearcher'.
				if (!type.IsAssignableTo(typeof(IStepSearcher)))
				{
					continue;
				}

				// The step searcher must contain a parameterless instance constructor.
				if (type.GetConstructors().All(static c => c.GetParameters().Length != 0))
				{
					continue;
				}

				// Now checks whether the step searcher can be separated into multiple instances.
				// If so, we should create instances one by one, and assign the properties with values
				// using the values inside the type 'SeparatedStepSearcherAttribute'.
				switch (type.GetCustomAttributes<SeparatedStepSearcherAttribute>().ToArray())
				{
					case { Length: not 0 } optionalAssignments:
					{
						// Sort the attribute instances via the priority.
						Array.Sort(optionalAssignments, static (x, y) => x.Priority.CompareTo(y.Priority));

						// Iterate on each attribute instances.
						foreach (var attributeInstance in optionalAssignments)
						{
							// Creates an instance.
							var instance = (IStepSearcher)Activator.CreateInstance(type)!;

							// Checks the inner values, in order to be used later.
							var propertyNamesAndValues = attributeInstance.PropertyNamesAndValues;
							switch (propertyNamesAndValues.Length)
							{
								case var length when (length & 1) != 0:
								{
									t("The property value is invalid.");
									break;
								}
								case var length:
								{
									for (var i = 0; i < length; i += 2)
									{
										var propertyName = (string)propertyNamesAndValues[i];
										var propertyValue = propertyNamesAndValues[i + 1];

										(
											type.GetProperty(propertyName) switch
											{
												null => t("Such property name cannot be found."),
												{ CanWrite: false } => t("The property is read-only and cannot be assigned."),
												var p => p
											}
										).SetValue(instance, propertyValue);
									}

									instance.Options = instance.Options with
									{
										SeparatedStepSearcherPriority = attributeInstance.Priority
									};

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


					[DoesNotReturn]
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					static PropertyInfo? t(string s) => throw new InvalidOperationException(s);
				}
			}

			// Assign the result.
			listOfStepSearchers.Sort(priorityComparison);

			return listOfStepSearchers.ToArray();


			static int priorityComparison(IStepSearcher s1, IStepSearcher s2)
			{
				if (s1.Options.Priority > s2.Options.Priority)
				{
					return 1;
				}
				else if (s1.Options.Priority < s2.Options.Priority)
				{
					return -1;
				}
				else if (s1.Options.SeparatedStepSearcherPriority > s2.Options.SeparatedStepSearcherPriority)
				{
					return 1;
				}
				else if (s1.Options.SeparatedStepSearcherPriority < s2.Options.SeparatedStepSearcherPriority)
				{
					return -1;
				}

				return 0;
			}
		}
	}
}
