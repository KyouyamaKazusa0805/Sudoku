namespace Sudoku.SourceGeneration.Handlers;

/// <summary>
/// Represents a file-local constraint for generators,
/// which can be used for <see cref="IncrementalGeneratorInitializationContext.SyntaxProvider"/>,
/// with <see cref="SyntaxValueProvider.ForAttributeWithMetadataName{T}(string, Func{SyntaxNode, CancellationToken, bool}, Func{GeneratorAttributeSyntaxContext, CancellationToken, T})"/>.
/// </summary>
/// <typeparam name="T">The type of the final data structure.</typeparam>
/// <seealso cref="IncrementalGeneratorInitializationContext.SyntaxProvider"/>
/// <seealso cref="SyntaxValueProvider.ForAttributeWithMetadataName{T}(string, Func{SyntaxNode, CancellationToken, bool}, Func{GeneratorAttributeSyntaxContext, CancellationToken, T})"/>
internal interface IIncrementalGeneratorAttributeHandler<T> where T : notnull
{
	/// <summary>
	/// Transform the target result from the specified <see cref="SyntaxNode"/> and its semantic values.
	/// </summary>
	/// <param name="gasc">The context used for getting basic information for a <see cref="SyntaxNode"/>.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel generating.</param>
	/// <returns>The result. The value can be <see langword="null"/>.</returns>
	public abstract T? Transform(GeneratorAttributeSyntaxContext gasc, CancellationToken cancellationToken);

	/// <summary>
	/// Try to generate the source.
	/// </summary>
	/// <param name="spc">The context used for generating.</param>
	/// <param name="values">The values.</param>
	public abstract void Output(SourceProductionContext spc, ImmutableArray<T> values);
}
