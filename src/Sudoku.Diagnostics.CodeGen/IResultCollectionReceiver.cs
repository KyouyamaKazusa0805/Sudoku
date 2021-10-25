namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// Defines a result collection receiver.
/// </summary>
/// <typeparam name="T">The type of each result value.</typeparam>
public interface IResultCollectionReceiver<T> : ISyntaxContextReceiver
{
	/// <summary>
	/// Indicates the result collection.
	/// </summary>
	ICollection<T> Collection { get; }
}
