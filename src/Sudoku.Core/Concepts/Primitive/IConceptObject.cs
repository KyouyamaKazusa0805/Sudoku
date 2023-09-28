namespace Sudoku.Concepts.Primitive;

/// <summary>
/// Represents an object type that describes for an object that is defined in sudoku concepts.
/// </summary>
/// <typeparam name="TSelf">The type of implementation.</typeparam>
/// <typeparam name="TConverter">The type of the converter.</typeparam>
public interface IConceptObject<TSelf, TConverter>
	where TSelf : IConceptObject<TSelf, TConverter>
	where TConverter : ISpecifiedConceptConverter<TSelf>
{
	/// <summary>
	/// Formats the current instance, converting it into a <see cref="string"/> result that can describe for this object.
	/// The conversion rule is specified as parameter <paramref name="converter"/>.
	/// </summary>
	/// <param name="converter">The coordinate converter object.</param>
	/// <returns>A <see cref="string"/> representation of the current object.</returns>
	public abstract string ToString(TConverter converter);
}
