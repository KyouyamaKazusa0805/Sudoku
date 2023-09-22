namespace Sudoku.Text.Coordinate;

/// <summary>
/// Defines a type that supports for coordinate output rule.
/// </summary>
public interface ICoordinateObject
{
	/// <summary>
	/// Formats the current instance, converting it into a <see cref="string"/> result that can describe for this object.
	/// The conversion rule is specified as parameter <paramref name="coordinateConverter"/>.
	/// </summary>
	/// <param name="coordinateConverter">The coordinate converter object.</param>
	/// <returns>A <see cref="string"/> representation of the current object.</returns>
	public abstract string ToString(CoordinateConverter coordinateConverter);
}
