using Sudoku.Concepts.Converters;

namespace Sudoku.Concepts.Primitive;

/// <summary>
/// Defines a type that supports for coordinate output rule.
/// </summary>
public interface ICoordinateObject
{
	/// <summary>
	/// Formats the current instance, converting it into a <see cref="string"/> result that can describe for this object.
	/// The conversion rule is specified as parameter <paramref name="converter"/>.
	/// </summary>
	/// <param name="converter">The coordinate converter object.</param>
	/// <returns>A <see cref="string"/> representation of the current object.</returns>
	public abstract string ToString(CoordinateConverter converter);
}
