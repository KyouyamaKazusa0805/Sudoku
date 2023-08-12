namespace Sudoku.Text.Notation;

/// <summary>
/// <inheritdoc cref="INotation" path="/summary"/>
/// </summary>
/// <typeparam name="TSelf">The type of the implementation.</typeparam>
/// <typeparam name="TElement">The type of the element after or before parsing.</typeparam>
public interface INotation<TSelf, TElement> : INotation where TSelf : notnull, INotation<TSelf, TElement> where TElement : notnull
{
	/// <summary>
	/// Gets the text notation that can represent the specified value.
	/// </summary>
	/// <param name="value">The instance to be output.</param>
	/// <returns>The string notation of the value.</returns>
	public static abstract string ToString(TElement value);
}
