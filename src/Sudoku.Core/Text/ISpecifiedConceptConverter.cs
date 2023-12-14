namespace Sudoku.Text;

/// <summary>
/// Represents a converter that only converts from a specified concept object into an equivalent <see cref="string"/> text.
/// </summary>
/// <typeparam name="T">The type of the instance to be converted.</typeparam>
public interface ISpecifiedConceptConverter<T>
{
	/// <summary>
	/// Indicates the converter that can convert a <typeparamref name="T"/> instance into an equivalent <see cref="string"/> representation.
	/// </summary>
	public abstract FuncRefReadOnly<T, string> Converter { get; }
}
