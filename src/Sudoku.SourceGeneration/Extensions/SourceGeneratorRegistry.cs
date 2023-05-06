namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides some registering methods for source generator context.
/// </summary>
internal static class SourceGeneratorRegistry
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
	public static void Register<THandler, TCollectedResult>(
		this scoped ref IncrementalGeneratorInitializationContext @this,
		string attributeName,
		Func<SyntaxNode, CancellationToken, bool> nodeFilter
	)
		where THandler : IIncrementalGeneratorAttributeHandler<TCollectedResult>, new()
		where TCollectedResult : class
	{
		var inst = new THandler();
		@this.RegisterSourceOutput(
			@this.SyntaxProvider
				.ForAttributeWithMetadataName(attributeName, nodeFilter, inst.Transform)
				.Where(NotNullPredicate)
				.Select(NotNullSelector)
				.Collect(),
			inst.Output
		);
	}

	/// <summary>
	/// Registers for a new generator function via attribute checking.
	/// </summary>
	/// <typeparam name="T1">The first type of the collected result.</typeparam>
	/// <typeparam name="T2">The second type of the collected result.</typeparam>
	/// <typeparam name="T3">The third type of the collected result.</typeparam>
	/// <param name="this">The context.</param>
	/// <param name="attributeName">
	/// The attribute name. The value must be full name of the attribute, including its namespace, beginning with root-level one.
	/// </param>
	/// <param name="nodeFilter">The node filter method.</param>
	/// <param name="transform1">The transform method that creates a nullable <typeparamref name="T1"/> instance.</param>
	/// <param name="transform2">The transform method that creates a nullable <typeparamref name="T2"/> instance.</param>
	/// <param name="transform3">The transform method that creates a nullable <typeparamref name="T3"/> instance.</param>
	/// <param name="output">The output method.</param>
	public static void Register<T1, T2, T3>(
		this scoped ref IncrementalGeneratorInitializationContext @this,
		string attributeName,
		Func<SyntaxNode, CancellationToken, bool> nodeFilter,
		Func<GeneratorAttributeSyntaxContext, CancellationToken, T1?> transform1,
		Func<GeneratorAttributeSyntaxContext, CancellationToken, T2?> transform2,
		Func<GeneratorAttributeSyntaxContext, CancellationToken, T3?> transform3,
		Action<SourceProductionContext, (T1[], T2[], T3[])> output
	)
		where T1 : class
		where T2 : class
		where T3 : class
		=> @this.RegisterSourceOutput(
			@this.SyntaxProvider
				.ForAttributeWithMetadataName(attributeName, nodeFilter, transform1)
				.Where(NotNullPredicate)
				.Select(NotNullSelector)
				.Collect()
				.Combine(
					@this.SyntaxProvider
						.ForAttributeWithMetadataName(attributeName, nodeFilter, transform2)
						.Where(NotNullPredicate)
						.Select(NotNullSelector)
						.Collect()
						.Combine(
							@this.SyntaxProvider
								.ForAttributeWithMetadataName(attributeName, nodeFilter, transform3)
								.Where(NotNullPredicate)
								.Select(NotNullSelector)
								.Collect()
						)
				)
				.Select(static (v, _) => (v.Left.ToArray(), v.Right.Left.ToArray(), v.Right.Right.ToArray())),
			output
		);

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
	public static void Register<THandler, TCollectedResult>(
		this scoped ref IncrementalGeneratorInitializationContext @this,
		string attributeName,
		string projectName,
		Func<SyntaxNode, CancellationToken, bool> nodeFilter
	)
		where THandler : IIncrementalGeneratorAttributeHandler<TCollectedResult>, new()
		where TCollectedResult : class
	{
		var inst = new THandler();
		@this.RegisterSourceOutput(
			@this.SyntaxProvider
				.ForAttributeWithMetadataName(attributeName, nodeFilter, inst.Transform)
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
	public static void Register<THandler>(this scoped ref IncrementalGeneratorInitializationContext @this, string projectName)
		where THandler : IIncrementalGeneratorCompilationHandler, new()
	{
		var inst = new THandler();
		@this.RegisterSourceOutput(@this.CompilationProvider, (spc, c) => { if (c.AssemblyName == projectName) { inst.Output(spc, c); } });
	}
}
