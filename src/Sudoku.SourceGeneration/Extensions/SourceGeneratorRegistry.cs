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
				.ForAttributeWithMetadataName(attributeName, (node, cancellationToken) => nodeFilter(node, cancellationToken), inst.Transform)
				.Where(NotNullPredicate)
				.Select(NotNullSelector)
				.Collect(),
			inst.Output
		);
	}

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
	{
		@this.RegisterSourceOutput(
			@this.SyntaxProvider
				.ForAttributeWithMetadataName(attributeName, (n, c) => nodeFilter(n, c), (c, ct) => transform1(c, ct))
				.Where(NotNullPredicate)
				.Select(NotNullSelector)
				.Collect()
				.Combine(
					@this.SyntaxProvider
						.ForAttributeWithMetadataName(attributeName, (n, c) => nodeFilter(n, c), (c, ct) => transform2(c, ct))
						.Where(NotNullPredicate)
						.Select(NotNullSelector)
						.Collect()
						.Combine(
							@this.SyntaxProvider
								.ForAttributeWithMetadataName(attributeName, (n, c) => nodeFilter(n, c), (c, ct) => transform3(c, ct))
								.Where(NotNullPredicate)
								.Select(NotNullSelector)
								.Collect()
						)
				)
				.Select(static (v, _) => (v.Left.ToArray(), v.Right.Left.ToArray(), v.Right.Right.ToArray())),
			(c, d) => output(c, d)
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
	public static void Register<THandler>(this scoped ref IncrementalGeneratorInitializationContext @this, string projectName)
		where THandler : IIncrementalGeneratorCompilationHandler, new()
	{
		var inst = new THandler();
		@this.RegisterSourceOutput(@this.CompilationProvider, (spc, c) => { if (c.AssemblyName == projectName) { inst.Output(spc, c); } });
	}
}
