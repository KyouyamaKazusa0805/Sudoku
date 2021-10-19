namespace Sudoku.CodeGenerating;

/// <summary>
/// Defines a set of members that source generators must implement.
/// </summary>
/// <typeparam name="TStruct">
/// The type of the gathered result. The type must be a <see cref="ValueTuple"/>.
/// </typeparam>
/// <remarks>
/// All those members in this type should be implemented explicitly in order not to expose to outside.
/// </remarks>
/// <seealso cref="ValueTuple"/>
internal interface ISyntaxHandling<TStruct> where TStruct : struct
{
	/// <summary>
	/// The method is invoked while pre-filtering.
	/// </summary>
	/// <param name="n">The syntax node.</param>
	/// <param name="c">The cancellation token that can cancel the operation.</param>
	/// <returns>
	/// A <see cref="bool"/> result indicating whether the syntax node satisfies the requirements.
	/// </returns>
	bool WithAttributes(SyntaxNode n, CancellationToken c);

	/// <summary>
	/// The method is invoked while transforming from a syntax context instance
	/// to a result information instance of type <typeparamref name="TStruct"/>.
	/// </summary>
	/// <param name="gsc">
	/// The specified generator syntax context. The context instance contains the syntax node itself,
	/// and the semantic model that allows the syntax node converting to a symbol,
	/// or getting operations from the syntax node itself.
	/// </param>
	/// <param name="c">The cancellation token that can cancel the operation.</param>
	/// <returns>The gathered information result instance.</returns>
	TStruct? Transform(GeneratorSyntaxContext gsc, CancellationToken c);

	/// <summary>
	/// The method is invoked while producing a source code file.
	/// </summary>
	/// <param name="spc">The context that can output the source files.</param>
	/// <param name="gathered">The gathered information result instance.</param>
	void SourceProduce(SourceProductionContext spc, TStruct? gathered);
}
