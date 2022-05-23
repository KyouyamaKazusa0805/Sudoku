namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Defines the type that stores the help options.
/// </summary>
[RootCommand("help", "Displays all possible root commands provided.", IsSpecial = true)]
[SupportedArguments(new[] { "help", "?" })]
[Usage("help", IsPattern = true)]
public sealed class Help : IExecutable
{
	/// <summary>
	/// Indicates the name of the help command. If <see langword="null"/>, all commands will be displayed.
	/// </summary>
	[SingleArgumentCommand("type-name", "Indicates the type name whose corresponding command introduction is what you want to see.")]
	public string? HelpCommandName { get; set; }


	/// <inheritdoc/>
	public void Execute()
	{
		if (typeof(Version).Assembly.GetName() is not { Name: { } realName, Version: var version })
		{
			// Returns an error that cannot fetch the assembly name correctly.
			throw new CommandLineRuntimeException((int)ErrorCode.AssemblyNameIsNull);
		}

		var thisType = GetType();
		var thisAssembly = thisType.Assembly;

		// Defines a builder that stores the content in order to output.
		var helpTextContentBuilder = new StringBuilder($"Project {realName}\r\nVersion {version}")
			.AppendLine()
			.AppendLine();

		if (HelpCommandName is null)
		{
			// Iterates on each command type, to get the maximum length of the command,
			// in order to display the commands alignedly.
			var commandTypes = thisAssembly.GetDerivedTypes(typeof(IExecutable));
			int maxWidth = 0;
			var listOfDescriptionParts = new List<(string CommandName, IEnumerable<string> DescriptionRawParts)>();
			foreach (var commandType in commandTypes)
			{
				var commandAttribute = commandType.GetCustomAttribute<RootCommandAttribute>()!;

				string commandName = commandAttribute.Name;
				maxWidth = Max(commandName.Length, maxWidth);

				var parts = commandAttribute.Description.SplitByLength(Console.LargestWindowWidth);

				listOfDescriptionParts.Add((commandName, parts));
			}

			// Build the content.
			helpTextContentBuilder.AppendLine("Root commands:").AppendLine();
			foreach (var (commandName, parts) in listOfDescriptionParts)
			{
				helpTextContentBuilder
					.Append(commandName.PadLeft(maxWidth + 1))
					.Append(new string(' ', 3))
					.AppendLine(string.Join($"\r\n{new string(' ', 3 + maxWidth)}", parts))
					.AppendLine();
			}

			// Output the value.
			Terminal.Write(helpTextContentBuilder.ToString());
		}
		else
		{
			// Iterates on each root command type, to validate whether the property points to the type.
			IExecutable? instance = null;
			foreach (var type in thisType.Assembly.GetDerivedTypes(typeof(IExecutable)))
			{
				var propertyInfo = thisType.GetProperty(nameof(HelpCommandName))!;
				var attribute = propertyInfo.GetCustomAttribute<SingleArgumentCommandAttribute>()!;
				var comparisonOption = attribute.IgnoreCase
					? StringComparison.OrdinalIgnoreCase
					: StringComparison.Ordinal;
				if (type.Name.Equals(HelpCommandName, comparisonOption))
				{
					instance = !type.Name.Equals(nameof(Help), comparisonOption)
						? (IExecutable)Activator.CreateInstance(type)!
						: throw new CommandLineRuntimeException((int)ErrorCode.TypeCannotBeHelp);
					break;
				}
			}

			// If none found, just throw exception to report the wrong case.
			if (instance is null)
			{
				throw new CommandLineRuntimeException((int)ErrorCode.TypeCannotBeFound);
			}

			// Display usage text.
			var instanceType = instance.GetType();
			var usageAttributes = instanceType.GetCustomAttributes<UsageAttribute>().ToArray();
			if (usageAttributes is [{ IsPattern: var firstIsPattern }, .. { Length: var otherArgsCount }])
			{
				// Output the title.
				helpTextContentBuilder.AppendLine("Syntax & Usage:").AppendLine();

				// Determines whether the collection has more than one pattern syntax usages.
				if (Array.FindAll(usageAttributes, static e => e.IsPattern).Length >= 2)
				{
					throw new CommandLineRuntimeException((int)ErrorCode.MultipleSyntaxPatternUsagesFound);
				}

				// Sort the attributes, and put the pattern syntax as the first place always.
				if (!firstIsPattern)
				{
					for (int i = 1; i < otherArgsCount + 1; i++)
					{
						if (usageAttributes[i].IsPattern)
						{
							(usageAttributes[i], usageAttributes[0]) = (usageAttributes[0], usageAttributes[i]);
							break;
						}
					}
				}

				// Then output the details.
				foreach (var usageAttribute in usageAttributes)
				{
					string patternSyntax = usageAttribute.ExampleCommand;
					var parts = usageAttribute.Description?.SplitByLength(Console.LargestWindowWidth);
					if (usageAttribute.IsPattern)
					{
						helpTextContentBuilder.AppendLine($"{new string(' ', 4)}{patternSyntax}").AppendLine();
					}
					else if (parts is not null)
					{
						helpTextContentBuilder
							.AppendLine($"{new string(' ', 4)}{patternSyntax}")
							.AppendLine(string.Concat(from part in parts select $"{new string(' ', 8)}{part}"))
							.AppendLine();
					}
				}

				helpTextContentBuilder.AppendLine();
			}

			List<(string CommandName, IEnumerable<string> DescriptionRawParts)>
				singleArguments = new(),
				doubleArguments = new();

			// Gets and iterates on all possible commands of the instance type.
			foreach (var (type, l1, l2) in
				from property in instanceType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
				where property is { CanRead: true, CanWrite: true }
				let l1 = property.GetCustomAttribute<SingleArgumentCommandAttribute>()
				let l2 = property.GetCustomAttribute<DoubleArgumentsCommandAttribute>()
				where l1 is not null || l2 is not null
				let rootCommandAttribute = instance.GetType().GetCustomAttribute<RootCommandAttribute>()
				where rootCommandAttribute is not null
				select (Type: rootCommandAttribute, L1: l1, L2: l2))
			{
				switch (l1, l2)
				{
					case ({ Notation: var notation, Description: var description, IsRequired: var isRequired }, null):
					{
						// l1 is not null
						singleArguments.Add(
							(
								$"<{notation}>",
								description.SplitByLength(Console.LargestWindowWidth - 4)
							)
						);

						break;
					}
#pragma warning disable IDE0055
					case (
						null,
						{
							FullName: var fullName,
							ShortName: var shortName,
							Description: var description,
							IsRequired: var isRequired
						}
					):
#pragma warning restore IDE0055
					{
						// l2 is not null
						var config = thisAssembly.GetCustomAttribute<GlobalConfigurationAttribute>() ?? new();
						string fullCommandNameSuffix = config.FullCommandNamePrefix;
						string shortCommandNameSuffix = config.ShortCommandNamePrefix;
						string fullCommand = $"{fullCommandNameSuffix}{fullName}";
						string shortCommand = $"{shortCommandNameSuffix}{shortName}";

						doubleArguments.Add(
							(
								$"{fullCommand} ({shortCommand}){(isRequired ? ", required" : string.Empty)}",
								description.SplitByLength(Console.LargestWindowWidth - 4)
							)
						);

						break;
					}
					default:
					{
						throw new CommandLineRuntimeException((int)ErrorCode.CommandArgumentCannotBeMultipleKind);
					}
				}
			}

			// Build the content.
			helpTextContentBuilder.AppendLine("Commands:").AppendLine();
			foreach (var (commandName, parts) in singleArguments)
			{
				helpTextContentBuilder
					.AppendLine($"{new string(' ', 4)}{commandName}")
					.AppendLine(string.Concat(from part in parts select $"{new string(' ', 8)}{part}"))
					.AppendLine();
			}
			foreach (var (commandName, parts) in doubleArguments)
			{
				helpTextContentBuilder
					.AppendLine($"{new string(' ', 4)}{commandName}")
					.AppendLine(string.Concat(from part in parts select $"{new string(' ', 8)}{part}"))
					.AppendLine();
			}

			// Output the value.
			Terminal.Write(helpTextContentBuilder.ToString());
		}
	}
}
