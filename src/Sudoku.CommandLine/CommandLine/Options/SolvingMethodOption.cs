namespace Sudoku.CommandLine.Options;

/// <summary>
/// Represents a solving method option.
/// </summary>
public sealed class SolvingMethodOption : Option<ISolver>, IOption<ISolver>
{
	/// <summary>
	/// Indicates the backing method map.
	/// </summary>
	private static readonly FrozenDictionary<string, Func<ISolver>> MethodMap = CreateMethodMap();


	/// <summary>
	/// Initializes a <see cref="SolvingMethodOption"/> instance.
	/// </summary>
	public SolvingMethodOption() : base(["--method", "-m"], ParseArgument, false, "Specifies the solving method")
	{
		Arity = ArgumentArity.ExactlyOne;
		IsRequired = false;
		SetDefaultValueFactory(static () => new BitwiseSolver());
	}


	/// <inheritdoc/>
	static ISolver IOptionOrArgument<ISolver>.ParseArgument(ArgumentResult result) => ParseArgument(result);

	/// <inheritdoc cref="IOptionOrArgument{T}.ParseArgument"/>
	private static ISolver ParseArgument(ArgumentResult result)
	{
		var token = result.Tokens is [{ Value: var f }] ? f : null;
		if (token is null)
		{
			result.ErrorMessage = "Argument expected.";
			return null!;
		}

		var names = string.Join(", ", MethodMap.Keys);
		if (!MethodMap.TryGetValue(token, out var solverCreator))
		{
			result.ErrorMessage = $"Invalid token. The expected values are {names}.";
			return null!;
		}

		return solverCreator();
	}

	/// <summary>
	/// To create solver field.
	/// </summary>
	/// <returns>The solver field.</returns>
	private static FrozenDictionary<string, Func<ISolver>> CreateMethodMap()
	{
		const string errorInfo_ConditionalPropertySetterValuesLengthMustBeEven = "Conditional property setter values invalid - the length of property must be an even number.";
		const string errorInfo_ConditionalPropertySetterValuesFirstArgumentMustBeString = "Condition property setter values invalid - the values at odd indices must be of type string.";

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
			foreach (var attribute in type.GetCustomAttributes<CommandBoundTypeAttribute>())
			{
				var validNames = attribute.Names;
				var conditionalPropertySetterValues = attribute.ConditionalPropertySetterValues ?? [];
				if ((conditionalPropertySetterValues.Length & 1) != 0)
				{
					throw new TypeInitializationException(
						typeof(SolvingMethodOption).FullName,
						new InvalidOperationException(errorInfo_ConditionalPropertySetterValuesLengthMustBeEven)
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
								new InvalidOperationException(errorInfo_ConditionalPropertySetterValuesFirstArgumentMustBeString)
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
