namespace System.CommandLine;

/// <summary>
/// Provides with the entry for the parsing.
/// </summary>
internal static class RootCommand
{
	/// <summary>
	/// Defines an event that is triggered when an operation is cancelled.
	/// </summary>
	public static event EventHandler? OperationCancelled;


	/// <summary>
	/// Routes the command line arguments to the specified <see cref="IExecutable"/> instances.
	/// </summary>
	/// <param name="args">The arguments.</param>
	/// <returns>A task that handles the operation asynchronously.</returns>
	/// <exception cref="CommandLineParserException">
	/// Throws when the command line arguments is <see langword="null"/> or empty currently,
	/// or the command name is invalid, or the command line arguments is empty.
	/// </exception>
	/// <exception cref="CommandLineRuntimeException">Throws when an error has been encountered.</exception>
	public static async Task RouteAsync(string[] args)
	{
		if (args is not [var rootCommand, ..])
		{
			throw new CommandLineParserException(CommandLineInternalError.ArgumentIsEmpty);
		}

		var type = (
			from t in typeof(Program).Assembly.GetTypes()
			where t.IsAssignableTo(typeof(IExecutable)) && t is { IsClass: true, IsAbstract: false }
			let parameterlessConstructorInfo = t.GetConstructor(Array.Empty<Type>())
			where parameterlessConstructorInfo is not null
			let attribute = t.GetCustomAttribute<SupportedArgumentsAttribute>()
			where attribute is not null
			let propertyValue = attribute.SupportedArguments
			let option = attribute.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal
			where propertyValue is not null && propertyValue.Any(s => s.Equals(rootCommand, option))
			select t
		).Single();

		var rootCommandInstance = (IExecutable)Activator.CreateInstance(type)!;
		Parser.ParseAndApplyTo(args, rootCommandInstance);

		try
		{
			var cts = new CancellationTokenSource();
			await rootCommandInstance.ExecuteAsync(cts.Token);
		}
		catch (OperationCanceledException)
		{
			OperationCancelled?.Invoke(null, EventArgs.Empty);
		}
	}
}
