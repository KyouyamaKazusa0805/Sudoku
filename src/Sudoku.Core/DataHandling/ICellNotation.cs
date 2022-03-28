namespace Sudoku.DataHandling;

/// <summary>
/// Defines a notation instance that can convert the <see cref="Cells"/> to the target <see cref="string"/>
/// representation that describes the instance, using the current notation rule.
/// </summary>
public interface ICellNotation : ISimpleParseable<Cells>
{
	/// <summary>
	/// Indicates the current name of the notation.
	/// </summary>
	static abstract string Name { get; }


	/// <summary>
	/// Gets the <see cref="string"/> representation of the current notation
	/// of the specified <see cref="Cells"/> instance.
	/// </summary>
	/// <param name="cells">
	/// The list of cells to be converted to the <see cref="string"/> representation
	/// of the current notation.
	/// </param>
	/// <returns>The <see cref="string"/> representation of the current notation.</returns>
	static abstract string ToDisplayString(in Cells cells);
}
