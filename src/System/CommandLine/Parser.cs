namespace System.CommandLine;

/// <summary>
/// Defines a command line parser that can parse the command line arguments as real instances.
/// </summary>
public static class Parser
{
	/// <summary>
	/// Try to parse the command line arguments and apply to the options into the specified instance.
	/// </summary>
	/// <param name="commandLineArguments">Indicates the command line arguments.</param>
	/// <param name="rootCommand">The option instance that stores the options.</param>
	/// <remarks>
	/// Due to using reflection, the type argument must be a <see langword="class"/> in order to prevent
	/// potential boxing and unboxing operations, which will make an unexpected error that the assignment
	/// will always be failed on <see langword="struct"/> types.
	/// </remarks>
	/// <exception cref="CommandLineParserException">
	/// Throws when the command line arguments is <see langword="null"/> or empty currently,
	/// or the command name is invalid.
	/// </exception>
	public static void ParseAndApplyTo(string[] commandLineArguments, IExecutable rootCommand)
	{
		var typeOfRootCommand = rootCommand.GetType();
		string[] supportedArguments = getArgs(typeOfRootCommand, out var comparisonOption);

		// Get all required properties.
		var listOfRequiredProperties = (
			from property in typeOfRootCommand.GetProperties(BindingFlags.Instance | BindingFlags.Public)
			where property is { CanRead: true, CanWrite: true }
			select property
		).ToList();

		// Checks the validity of the command line arguments.
		if (commandLineArguments is not [var possibleCommandName, .. var otherArgs])
		{
			throw new CommandLineParserException(CommandLineInternalError.ArgumentFormatInvalid);
		}

		// Checks whether the current command line name matches the specified one.
		if (!supportedArguments.Any(e => rootCommandMatcher(possibleCommandName, e)))
		{
			throw new CommandLineParserException(CommandLineInternalError.CommandNameIsInvalid);
		}

		// Now gets the information of the global configuration.
		var targetAssembly = typeOfRootCommand.Assembly;
		var globalOptions = targetAssembly.GetCustomAttribute<GlobalConfigurationAttribute>() ?? new();

		// Checks for each argument of type string, and assigns the value using reflection.
		int i = 0;
		while (i < otherArgs.Length)
		{
			// Gets the name of the command.
			string currentArg = otherArgs[i];
			switch (globalOptions)
			{
				case { FullCommandNamePrefix: var fullCommandNamePrefix }
				when currentArg.StartsWith(fullCommandNamePrefix) && currentArg.Length > fullCommandNamePrefix.Length:
				{
					// Okay. Long name.
					string realSubcommand = currentArg[fullCommandNamePrefix.Length..];

					// Then find property in the type.
					var properties = (
						from propertyInfo in typeOfRootCommand.GetProperties()
						where propertyInfo is { CanRead: true, CanWrite: true }
						let attribute = propertyInfo.GetCustomAttribute<DoubleArgumentsCommandAttribute>()
						where attribute?.FullName.Equals(realSubcommand, StringComparison.OrdinalIgnoreCase) ?? false
						select propertyInfo
					).ToArray();
					if (properties is not [{ PropertyType: var propertyType } property])
					{
						throw new CommandLineParserException(CommandLineInternalError.ArgumentsAmbiguousMatchedOrMismatched);
					}

					// Assign the real value.
					assignPropertyValue(property, propertyType, i + 1);

					// Advances the move.
					i += 2;

					break;
				}
				case { ShortCommandNamePrefix: var shortCommandNamePrefix }
				when currentArg.StartsWith(shortCommandNamePrefix) && currentArg.Length == shortCommandNamePrefix.Length + 1:
				{
					// Okay. Short name.
					char realSubcommand = currentArg[^1];

					// Then find property in the type.
					var properties = (
						from propertyInfo in typeOfRootCommand.GetProperties()
						where propertyInfo is { CanRead: true, CanWrite: true }
						let attribute = propertyInfo.GetCustomAttribute<DoubleArgumentsCommandAttribute>()
						where attribute?.ShortName == realSubcommand
						select propertyInfo
					).ToArray();
					if (properties is not [{ PropertyType: var propertyType } property])
					{
						throw new CommandLineParserException(CommandLineInternalError.ArgumentsAmbiguousMatchedOrMismatched);
					}

					// Assign the real value.
					assignPropertyValue(property, propertyType, i + 1);

					// Advances the move.
					i += 2;

					break;
				}
				default:
				{
					// Try to treat the argument as the single-argument command.
					var properties = (
						from propertyInfo in typeOfRootCommand.GetProperties()
						where propertyInfo is { CanRead: true, CanWrite: true }
						let attribute = propertyInfo.GetCustomAttribute<SingleArgumentCommandAttribute>()
						where attribute is not null
						select propertyInfo
					).ToArray();
					if (properties is not [{ PropertyType: var propertyType } property])
					{
						throw new CommandLineParserException(CommandLineInternalError.MultipleSingleArgumentCommandPropertiesFound);
					}

					// Assign the real value.
					assignPropertyValue(property, propertyType, i);

					// Advances the move.
					i++;

					break;
				}
			}

			// Checks whether all required properties are assigned explicitly.
			// If not, an exception will be thrown.
			if (listOfRequiredProperties.Count != 0)
			{
				string requiredPropertiesNotAssignedStr = string.Join(
					$"\r\n{new string(' ', 4)}",
					from propertyInfo in listOfRequiredProperties
					let name = propertyInfo.Name
					let attribute = propertyInfo.GetCustomAttribute<DoubleArgumentsCommandAttribute>()!
					let pair = (attribute.ShortName, attribute.FullName)
					select $"Command {name} (short: {pair.ShortName}, long: {pair.FullName})"
				);

				throw new CommandLineParserException(
					CommandLineInternalError.NotAllRequiredPropertiesAreAssigned,
					$"Required properties not assigned: {requiredPropertiesNotAssignedStr}"
				);
			}


			void assignPropertyValue(PropertyInfo property, Type propertyType, int argPos)
			{
				if (argPos >= otherArgs.Length)
				{
					throw new CommandLineParserException(CommandLineInternalError.ArgumentExpected);
				}

				// Converts the real argument value into the target property typed instance.
				string realValue = otherArgs[argPos];
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
							: throw new CommandLineParserException(CommandLineInternalError.ConvertedTypeMustBeString));
				}

				// Removes the value from the required property list.
				listOfRequiredProperties.RemoveAll(p => p == property);
			}
		}


		bool rootCommandMatcher(string c, string e) => e.Equals(c, comparisonOption);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string[] getArgs(Type typeOfRootCommand, out StringComparison comparisonOption)
		{
			var attribute = typeOfRootCommand.GetCustomAttribute<SupportedArgumentsAttribute>()!;
			comparisonOption = attribute.IgnoreCase
				? StringComparison.OrdinalIgnoreCase
				: StringComparison.Ordinal;

			return attribute.SupportedArguments;
		}
	}
}
