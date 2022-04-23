namespace System.CommandLine;

/// <summary>
/// Provides with the entry for the parsing.
/// </summary>
internal static class RootCommand
{
	/// <summary>
	/// Routes the command line arguments to the specified <see cref="IRootCommand"/> instances.
	/// </summary>
	/// <param name="args">The arguments.</param>
	/// <exception cref="CommandLineParserException">
	/// Throws when the command line arguments is <see langword="null"/> or empty currently,
	/// or the command name is invalid, or the command line arguments is empty.
	/// </exception>
	/// <exception cref="CommandLineRuntimeException">Throws when an error has been encountered.</exception>
	public static void Route(string[] args)
	{
		if (args is not [var rootCommand, ..])
		{
			throw new CommandLineParserException(CommandLineInternalError.ArgumentIsEmpty);
		}

		var type = (
			from t in typeof(Program).Assembly.GetTypes()
			where t.IsAssignableTo(typeof(IRootCommand)) && t is { IsClass: true, IsAbstract: false }
			let parameterlessConstructorInfo = t.GetConstructor(Array.Empty<Type>())
			where parameterlessConstructorInfo is not null
			let attribute = t.GetCustomAttribute<SupportedArgumentsAttribute>()
			where attribute is not null
			let propertyValue = attribute.SupportedArguments
			let option = attribute.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal
			where propertyValue is not null && propertyValue.Any(s => s.Equals(rootCommand, option))
			select t
		).Single();

		var rootCommandInstance = (IRootCommand)Activator.CreateInstance(type)!;
		Parser.ParseAndApplyTo(args, rootCommandInstance);
		rootCommandInstance.Execute();
	}
}
