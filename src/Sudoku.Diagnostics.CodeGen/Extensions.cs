namespace Sudoku.Diagnostics.CodeGen;

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
internal static class Extensions
{
	/// <summary>
	/// Registers for a new generator function via attribute checking.
	/// </summary>
	/// <typeparam name="THandler">
	/// The type of the target handler. The handler type must implement <see cref="IIncrementalGeneratorAttributeHandler{T}"/>,
	/// and contain a parameterless constructor.
	/// </typeparam>
	/// <typeparam name="TCollectedResult">
	/// The type of the collected result. The type must be as a generic type argument of <typeparamref name="THandler"/>.
	/// </typeparam>
	/// <param name="this">The context.</param>
	/// <param name="attributeName">
	/// The attribute name. The value must be full name of the attribute, including its namespace, beginning with root-level one.
	/// </param>
	/// <param name="nodeFilter">The node filter method.</param>
	/// <seealso cref="IIncrementalGeneratorAttributeHandler{T}"/>
	public static unsafe void Register<THandler, TCollectedResult>(
		this scoped ref IncrementalGeneratorInitializationContext @this,
		string attributeName,
		delegate*<SyntaxNode, CancellationToken, bool> nodeFilter
	)
		where THandler : IIncrementalGeneratorAttributeHandler<TCollectedResult>, new()
		where TCollectedResult : class
	{
		var inst = new THandler();
		@this.RegisterSourceOutput(
			@this.SyntaxProvider
				.ForAttributeWithMetadataName(attributeName, (node, cancellationToken) => nodeFilter(node, cancellationToken), inst.Transform)
				.Where(NotNullPredicate)
				.Select(NotNullSelector)
				.Collect(),
			inst.Output
		);
	}

	/// <summary>
	/// Registers for a new generator function via attribute checking.
	/// </summary>
	/// <typeparam name="THandler">
	/// The type of the target handler. The handler type must implement <see cref="IIncrementalGeneratorAttributeHandler{T}"/>,
	/// and contain a parameterless constructor.
	/// </typeparam>
	/// <typeparam name="TCollectedResult">
	/// The type of the collected result. The type must be as a generic type argument of <typeparamref name="THandler"/>.
	/// </typeparam>
	/// <param name="this">The context.</param>
	/// <param name="attributeName">
	/// The attribute name. The value must be full name of the attribute, including its namespace, beginning with root-level one.
	/// </param>
	/// <param name="projectName">The project name.</param>
	/// <param name="nodeFilter">The node filter method.</param>
	/// <seealso cref="IIncrementalGeneratorAttributeHandler{T}"/>
	public static unsafe void Register<THandler, TCollectedResult>(
		this scoped ref IncrementalGeneratorInitializationContext @this,
		string attributeName,
		string projectName,
		delegate*<SyntaxNode, CancellationToken, bool> nodeFilter
	)
		where THandler : IIncrementalGeneratorAttributeHandler<TCollectedResult>, new()
		where TCollectedResult : class
	{
		var inst = new THandler();
		@this.RegisterSourceOutput(
			@this.SyntaxProvider
				.ForAttributeWithMetadataName(attributeName, (node, cancellationToken) => nodeFilter(node, cancellationToken), inst.Transform)
				.Where(NotNullPredicate)
				.Select(NotNullSelector)
				.Combine(@this.CompilationProvider)
				.Where(pair => pair.Right.AssemblyName == projectName)
				.Select(static (pair, _) => pair.Left)
				.Collect(),
			inst.Output
		);
	}

	/// <summary>
	/// Registers for a new generator function via compilation.
	/// </summary>
	/// <typeparam name="THandler">The handler.</typeparam>
	/// <param name="this">The context.</param>
	/// <param name="projectName">The full name of the project that can filter compilation projects.</param>
	public static unsafe void Register<THandler>(this scoped ref IncrementalGeneratorInitializationContext @this, string projectName)
		where THandler : IIncrementalGeneratorCompilationHandler, new()
	{
		var inst = new THandler();
		@this.RegisterSourceOutput(
			@this.CompilationProvider,
			(spc, c) => { if (c.AssemblyName == projectName) { inst.Output(spc, c); } }
		);
	}

	/// <summary>
	/// Determines whether all parameters are <see langword="out"/> ones.
	/// </summary>
	/// <param name="this">A list of parameters.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool AllOutParameters(this ImmutableArray<IParameterSymbol> @this)
		=> @this.All(static parameter => parameter.RefKind == RefKind.Out);

	/// <summary>
	/// Internal handle the naming rule, converting it into a valid identifier via specified parameter name.
	/// </summary>
	/// <param name="this">The naming rule.</param>
	/// <param name="parameterName">The parameter name.</param>
	/// <returns>The final identifier.</returns>
	public static string InternalHandle(this string @this, string parameterName)
		=> @this
			.Replace("<@", parameterName.ToCamelCasing())
			.Replace(">@", parameterName.ToPascalCasing())
			.Replace("@", parameterName);

	/// <summary>
	/// Try to convert the specified identifier into pascal casing.
	/// </summary>
	public static string ToPascalCasing(this string @this) => $"{char.ToUpper(@this[0])}{@this[1..]}";

	/// <summary>
	/// Try to convert the specified identifier into camel casing.
	/// </summary>
	public static string ToCamelCasing(this string @this) => $"{char.ToLower(@this[0])}{@this[1..]}";
}
