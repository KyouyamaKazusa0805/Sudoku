namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// Defines a result collection receiver.
/// </summary>
/// <typeparam name="T">The type of each result value.</typeparam>
internal interface IResultCollectionReceiver<T>
{
	/// <summary>
	/// Indicates the result collection.
	/// </summary>
	ICollection<T> Collection { get; }
}
