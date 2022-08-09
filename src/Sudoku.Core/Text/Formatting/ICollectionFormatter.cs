namespace Sudoku.Text.Formatting;

/// <summary>
/// Defines a formatter that can create a <see cref="string"/> value representing the current instances
/// of type <typeparamref name="TElement"/>.
/// </summary>
/// <typeparam name="TElement">The type of the elements in the collection.</typeparam>
public interface ICollectionFormatter<in TElement>
{
	/// <summary>
	/// Try to format a list of <typeparamref name="TElement"/> instances, split by <paramref name="separator"/>s
	/// between two adjacent elements.
	/// </summary>
	/// <param name="elements">The elements to be formatted.</param>
	/// <param name="separator">
	/// The separator that will be inserted into each two adjacent elements in this collection.
	/// </param>
	/// <returns>A <see cref="string"/> result.</returns>
	public static abstract string Format(IEnumerable<TElement> elements, string separator);

	/// <summary>
	/// Try to format a list of <typeparamref name="TElement"/> instances using the specified formatting mode.
	/// </summary>
	/// <param name="elements">The elements to be formatted.</param>
	/// <param name="formattingMode">
	/// The formatting mode. The default value is <see cref="FormattingMode.Simple"/>.
	/// </param>
	/// <returns>A <see cref="string"/> result.</returns>
	public static abstract string Format(IEnumerable<TElement> elements, FormattingMode formattingMode = FormattingMode.Simple);
}
