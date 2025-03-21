namespace Sudoku;

/// <include file="../../global-doc-comments.xml" path="//g/csharp9/feature[@name='module-initializer']/target[@name='type']"/>
[EditorBrowsable(EditorBrowsableState.Never)]
internal static class ModuleInitializer
{
	/// <summary>
	/// Represents error message on "length must be even".
	/// </summary>
	private const string ErrorInfo_ConditionalPropertySetterValuesLengthMustBeEven = "Conditional property setter values invalid - the length of property must be an even number.";

	/// <summary>
	/// Represents error message on "first element must be string".
	/// </summary>
	private const string ErrorInfo_ConditionalPropertySetterValuesFirstArgumentMustBeString = "Condition property setter values invalid - the values at odd indices must be of type string.";


	/// <include file="../../global-doc-comments.xml" path="//g/csharp9/feature[@name='module-initializer']/target[@name='method']"/>
	[ModuleInitializer]
	public static void Initialize()
	{
		SolvingMethodOption.MethodMap = CreateSolverField();
	}


	/// <summary>
	/// To create solver field.
	/// </summary>
	/// <returns>The solver field.</returns>
	private static FrozenDictionary<string, Func<ISolver>> CreateSolverField()
	{
		var solverType = typeof(ISolver);
		var validTypes = new List<KeyValuePair<string, Func<ISolver>>>();
		foreach (var type in solverType.Assembly.GetTypes())
		{
			if (!type.IsAssignableTo(solverType))
			{
				continue;
			}

			// Check whether the object contains a parameterless constructor with public accessibility.
			if (!type.HasParameterlessConstructor())
			{
				continue;
			}

			// Check attribute.
			foreach (var attribute in type.GetCustomAttributes<InstancePropertyAttribute>())
			{
				var validNames = attribute.Names;
				var conditionalPropertySetterValues = attribute.ConditionalPropertySetterValues ?? [];
				if ((conditionalPropertySetterValues.Length & 1) != 0)
				{
					throw new TypeInitializationException(
						typeof(SolvingMethodOption).FullName,
						new InvalidOperationException(ErrorInfo_ConditionalPropertySetterValuesLengthMustBeEven)
					);
				}

				validTypes.AddRange(from name in validNames select KeyValuePair.Create(name, instanceCreation));


				ISolver instanceCreation()
				{
					var instance = (ISolver)Activator.CreateInstance(type)!;
					for (var i = 0; i < conditionalPropertySetterValues.Length; i += 2)
					{
						var a = conditionalPropertySetterValues[i];
						var b = conditionalPropertySetterValues[i + 1];
						if (a is not string fieldOrPropertyName)
						{
							throw new TypeInitializationException(
								typeof(SolvingMethodOption).FullName,
								new InvalidOperationException(ErrorInfo_ConditionalPropertySetterValuesFirstArgumentMustBeString)
							);
						}

						var fieldInfo = type.GetField(fieldOrPropertyName, BindingFlags.Public | BindingFlags.Instance);
						if (fieldInfo is null)
						{
							goto TryProperty;
						}

						fieldInfo.SetValue(instance, b);
						goto Next;

					TryProperty:
						var propertyInfo = type.GetProperty(fieldOrPropertyName, BindingFlags.Public | BindingFlags.Instance);
						if (propertyInfo is not { CanWrite: true })
						{
							// Failed to retrieve the target member.
							continue;
						}

						propertyInfo.SetValue(instance, b);
					Next:;
					}
					return instance;
				}
			}
		}

		return new Dictionary<string, Func<ISolver>>(validTypes, StringComparer.OrdinalIgnoreCase).ToFrozenDictionary();
	}
}
