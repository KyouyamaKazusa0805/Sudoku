namespace Sudoku.Concepts;

public partial interface IBitStatusMap<TSelf, TElement, TEnumerator>
{
	/// <summary>
	/// The file-local handler on <typeparamref name="TCollection"/> adding operation, with a new element.
	/// </summary>
	/// <typeparam name="TCollection">The type of collection.</typeparam>
	/// <param name="result">The result instance.</param>
	/// <param name="cells">The values to be added.</param>
	private protected delegate void CollectionAddingHandler<TCollection>(TCollection result, TElement[] cells);
}
