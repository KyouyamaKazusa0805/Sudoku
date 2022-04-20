namespace System.CommandLine;

/// <summary>
/// Defines a command line parser that can parse the command line arguments as real instances.
/// </summary>
/// <param name="Arguments">Indicates the command line arguments.</param>
public readonly record struct Parser(string[] Arguments) :
	IEquatable<Parser>,
	IEqualityOperators<Parser, Parser>
{
	/// <summary>
	/// Try to parse the command line arguments and apply to the options into the specified instance.
	/// </summary>
	/// <typeparam name="TRootCommand">The type of the instance.</typeparam>
	/// <typeparam name="TErrorCode">The type suggests the result.</typeparam>
	/// <param name="rootCommand">The option instance that stores the options.</param>
	/// <remarks>
	/// Due to using reflection, the type argument must be a <see langword="class"/> in order to prevent
	/// potential boxing and unboxing operations, which will make an unexpected error that the assignment
	/// will always be failed on <see langword="struct"/> types.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	/// Throws when the command line arguments is <see langword="null"/> or empty currently,
	/// or the command name is invalid.
	/// </exception>
	public void ParseAndApplyTo<TRootCommand, TErrorCode>(TRootCommand rootCommand)
		where TRootCommand : class, IRootCommand<TErrorCode>
		where TErrorCode : Enum
	{
		switch (rootCommand)
		{
			// Special case: If the type is the special one, just return.
			case ISpecialCommand<TErrorCode>:
			{
				if (predicate(Arguments))
				{
					return;
				}

				throw new InvalidOperationException("Don't require any command line arguments now.");
			}
			default:
			{
				break;
			}


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool predicate(string[] args) => args is [var c] && c == TRootCommand.Name;
		}

		// Checks the validity of the command line arguments.
		if (Arguments is not [var possibleCommandName, .. var otherArgs])
		{
			throw new InvalidOperationException("The command line arguments is invalid.");
		}

		// Checks whether the current command line name matches the specified one.
		bool rootCommandMatcher(string e) => e.Equals(possibleCommandName, StringComparison.OrdinalIgnoreCase);
		if (TRootCommand.SupportedCommands.Any(rootCommandMatcher))
		{
			throw new InvalidOperationException("The command name is invalid.");
		}

		// Now gets the information of the global configration.
		var targetAssembly = typeof(TRootCommand).Assembly;
		var globalOptions = targetAssembly.GetCustomAttribute<GlobalConfigurationAttribute>() ?? new();

		// Checks for each argument of type string, and assigns the value using reflection.
		int i = 0;
		while (i < otherArgs.Length)
		{
			// Gets the name of the command.
			string currentArg = otherArgs[i];
			if (globalOptions.FullCommandNamePrefix is var fullCommandNamePrefix
				&& currentArg.StartsWith(fullCommandNamePrefix)
				&& currentArg.Length > fullCommandNamePrefix.Length)
			{
				// Okay. Long name.
				string realSubcommand = currentArg[fullCommandNamePrefix.Length..];

				// Then find property in the type.
				var properties = (
					from propertyInfo in typeof(TRootCommand).GetProperties()
					where propertyInfo is { CanRead: true, CanWrite: true }
					let attribute = propertyInfo.GetCustomAttribute<CommandAttribute>()
					where attribute?.FullName.Equals(realSubcommand, StringComparison.OrdinalIgnoreCase) ?? false
					select propertyInfo
				).ToArray();
				if (properties is not [{ PropertyType: var propertyType } property])
				{
					throw new InvalidOperationException("Ambiguous matched or mismatched.");
				}

				// Assign the real value.
				assignPropertyValue(property, propertyType);

				// Advances the move.
				i += 2;
			}
			else if (
				globalOptions.ShortCommandNamePrefix is var shortCommandNamePrefix
				&& currentArg.StartsWith(shortCommandNamePrefix)
				&& currentArg.Length == shortCommandNamePrefix.Length + 1
			)
			{
				// Okay. Short name.
				char realSubcommand = currentArg[^1];

				// Then find property in the type.
				var properties = (
					from propertyInfo in typeof(TRootCommand).GetProperties()
					where propertyInfo is { CanRead: true, CanWrite: true }
					let attribute = propertyInfo.GetCustomAttribute<CommandAttribute>()
					where attribute?.ShortName == realSubcommand
					select propertyInfo
				).ToArray();
				if (properties is not [{ PropertyType: var propertyType } property])
				{
					throw new InvalidOperationException("Ambiguous matched or mismatched.");
				}

				// Assign the real value.
				assignPropertyValue(property, propertyType);

				// Advances the move.
				i += 2;
			}
			else
			{
				// Mismatched.
				throw new InvalidOperationException("The argument mismatched.");
			}


			void assignPropertyValue(PropertyInfo property, Type propertyType)
			{
				if (i + 1 >= otherArgs.Length)
				{
					throw new InvalidOperationException("Cannot operate due to the lack of the real value.");
				}

				// Converts the real argument value into the target property typed instance.
				string realValue = otherArgs[i + 1];
				var propertyConverterAttribute = property.GetCustomAttribute<CommandConverterAttribute>();
				if (propertyConverterAttribute is { ConverterType: var converterType })
				{
					// Creates a converter instance.
					var instance = (IValueConverter)Activator.CreateInstance(converterType)!;

					// Set the value to the property.
					property.SetValue(rootCommand, instance.Convert(realValue));
				}
				else
				{
					property.SetValue(
						rootCommand,
						propertyType == typeof(string)
							? realValue
							: throw new InvalidOperationException("The target type must be a string."));
				}
			}
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Parser other) => Enumerable.SequenceEqual(Arguments, other.Arguments);

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var hashCode = new HashCode();
		foreach (string argument in Arguments)
		{
			hashCode.Add(argument);
		}

		return hashCode.ToHashCode();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => string.Join(' ', Arguments);
}
