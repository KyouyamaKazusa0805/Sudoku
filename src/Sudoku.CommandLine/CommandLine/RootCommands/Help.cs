namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Defines the type that stores the help options.
/// </summary>
[RootCommand("help", DescriptionResourceKey = "_Description_Help", IsSpecial = true)]
[SupportedArguments("help", "?")]
[Usage("help", IsPattern = true)]
public sealed class Help : IExecutable
{
	/// <summary>
	/// Indicates the name of the help command. If <see langword="null"/>, all commands will be displayed.
	/// </summary>
	[SingleArgumentCommand("type-name", DescriptionResourceKey = "_Description_HelpCommandName_Help")]
	public string? HelpCommandName { get; set; }


	/// <inheritdoc/>
	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		if (typeof(Version).Assembly.GetName() is not { Name: { } realName, Version: var version })
		{
			// Returns an error that cannot fetch the assembly name correctly.
			throw new CommandLineRuntimeException((int)ErrorCode.AssemblyNameIsNull);
		}

		var thisType = GetType();
		var thisAssembly = thisType.Assembly;

		// Defines a builder that stores the content in order to output.
		var helpTextContentBuilder = new StringBuilder($"{GetString("_MessageFormat_Project")} {realName}\r\n{GetString("_MessageFormat_Version")} {version}\r\n\r\n");
		if (HelpCommandName is null)
		{
			// Iterates on each command type, to get the maximum length of the command,
			// in order to display the commands with characters aligned.
			var commandTypes = thisAssembly.GetDerivedTypes<IExecutable>();
			var maxWidth = 0;
			var listOfDescriptionParts = new List<(string CommandName, IEnumerable<string> DescriptionRawParts)>();
			foreach (var commandType in commandTypes)
			{
				switch (commandType.GetCustomAttribute<RootCommandAttribute>())
				{
					case { Name: { Length: var commandNameLength } commandName, DescriptionResourceKey: { } key }:
					{
						maxWidth = Max(commandNameLength, maxWidth);

						var parts = GetString(key)!.SplitByLength(Console.LargestWindowWidth);
						listOfDescriptionParts.Add((commandName, parts));

						break;
					}
					case { Name: { Length: var commandNameLength } commandName, Description: { } description }:
					{
						maxWidth = Max(commandNameLength, maxWidth);

						var parts = description.SplitByLength(Console.LargestWindowWidth);
						listOfDescriptionParts.Add((commandName, parts));

						break;
					}
				}
			}

			// Build the content.
			helpTextContentBuilder.AppendLine(GetString("_MessageFormat_RootCommandsAre")!).AppendLine();
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
			var instance = default(IExecutable);
			foreach (var type in thisType.Assembly.GetDerivedTypes<IExecutable>())
			{
				var propertyInfo = thisType.GetProperty(nameof(HelpCommandName))!;
				var attribute = propertyInfo.GetCustomAttribute<SingleArgumentCommandAttribute>()!;
				var comparisonOption = attribute.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
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
				helpTextContentBuilder.AppendLine(GetString("_MessageFormat_SyntaxAndUsageIs")!).AppendLine();

				// Determines whether the collection has more than one pattern syntax usages.
				if (Array.FindAll(usageAttributes, static e => e.IsPattern).Length >= 2)
				{
					throw new CommandLineRuntimeException((int)ErrorCode.MultipleSyntaxPatternUsagesFound);
				}

				// Sort the attributes, and put the pattern syntax as the first place always.
				if (!firstIsPattern)
				{
					for (var i = 1; i < otherArgsCount + 1; i++)
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
					switch (usageAttribute)
					{
						case { Example: var pattern, IsPattern: true }:
						{
							helpTextContentBuilder.AppendLine($"{new string(' ', 4)}{pattern}").AppendLine();
							break;
						}
						case { Example: var pattern, DescriptionResourceKey: { } key }
						when GetString(key)?.SplitByLength(Console.LargestWindowWidth) is { } parts:
						{
							helpTextContentBuilder
								.AppendLine($"{new string(' ', 4)}{pattern}")
								.AppendLine(string.Concat(from part in parts select $"{new string(' ', 8)}{part}"))
								.AppendLine();

							break;
						}
						case { Example: var pattern, Description: { } description }
						when description.SplitByLength(Console.LargestWindowWidth) is var parts:
						{
							helpTextContentBuilder
								.AppendLine($"{new string(' ', 4)}{pattern}")
								.AppendLine(string.Concat(from part in parts select $"{new string(' ', 8)}{part}"))
								.AppendLine();

							break;
						}
					}
				}

				helpTextContentBuilder.AppendLine();
			}

			var singleArguments = new List<(string CommandName, IEnumerable<string> DescriptionRawParts)>();
			var doubleArguments = new List<(string CommandName, IEnumerable<string> DescriptionRawParts)>();

			// Gets and iterates on all possible commands of the instance type.
			foreach (var (l1, l2) in
				from property in instanceType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
				where property is { CanRead: true, CanWrite: true }
				let l1 = property.GetCustomAttribute<SingleArgumentCommandAttribute>()
				let l2 = property.GetCustomAttribute<DoubleArgumentsCommandAttribute>()
				where l1 is not null || l2 is not null
				let rootCommandAttribute = instance.GetType().GetCustomAttribute<RootCommandAttribute>()
				where rootCommandAttribute is not null
				select (l1, l2))
			{
				switch (l1, l2)
				{
					case ({ Notation: var notation, DescriptionResourceKey: { } key, IsRequired: var isRequired }, null):
					{
						// l1 is not null
						singleArguments.Add(($"<{notation}>", GetString(key)!.SplitByLength(Console.LargestWindowWidth - 4)));

						break;
					}
					case ({ Notation: var notation, Description: { } description, IsRequired: var isRequired }, null):
					{
						// l1 is not null
						singleArguments.Add(($"<{notation}>", description.SplitByLength(Console.LargestWindowWidth - 4)));

						break;
					}
					case (null, { FullName: var fullName, ShortName: var shortName, DescriptionResourceKey: { } key, IsRequired: var isRequired }):
					{
						// l2 is not null
						var config = thisAssembly.GetCustomAttribute<GlobalConfigurationAttribute>() ?? new();
						var fullCommandNameSuffix = config.FullCommandNamePrefix;
						var shortCommandNameSuffix = config.ShortCommandNamePrefix;
						var fullCommand = $"{fullCommandNameSuffix}{fullName}";
						var shortCommand = $"{shortCommandNameSuffix}{shortName}";

						doubleArguments.Add(
							(
								$"{fullCommand} ({shortCommand}){(isRequired ? GetString("_MessageFormat_AndIsRequired")! : string.Empty)}",
								GetString(key)!.SplitByLength(Console.LargestWindowWidth - 4)
							)
						);

						break;
					}
					case (null, { FullName: var fullName, ShortName: var shortName, Description: { } description, IsRequired: var isRequired }):
					{
						// l2 is not null
						var config = thisAssembly.GetCustomAttribute<GlobalConfigurationAttribute>() ?? new();
						var fullCommandNameSuffix = config.FullCommandNamePrefix;
						var shortCommandNameSuffix = config.ShortCommandNamePrefix;
						var fullCommand = $"{fullCommandNameSuffix}{fullName}";
						var shortCommand = $"{shortCommandNameSuffix}{shortName}";

						doubleArguments.Add(
							(
								$"{fullCommand} ({shortCommand}){(isRequired ? GetString("_MessageFormat_AndIsRequired")! : string.Empty)}",
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
			helpTextContentBuilder.AppendLine(GetString("_MessageFormat_CommandsAre")!).AppendLine();
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
			await Terminal.WriteAsync(helpTextContentBuilder.ToString());
		}
	}
}
