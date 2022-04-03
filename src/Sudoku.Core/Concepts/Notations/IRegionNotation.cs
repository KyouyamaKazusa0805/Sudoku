namespace Sudoku.Concepts.Notations;

/// <summary>
/// Defines the notation that handles for the regions.
/// </summary>
public interface IRegionNotation : INotation
{
	/// <summary>
	/// Gets the string representation of the regions.
	/// </summary>
	/// <param name="regions">The regions.</param>
	/// <returns>The string representation of the regions.</returns>
	static abstract string ToRegionString(params int[] regions);

	/// <summary>
	/// Gets the shortcut of string as the representation of the regions.
	/// </summary>
	/// <param name="regions">The regions.</param>
	/// <returns>The string representation of the regions.</returns>
	static abstract string ToRegionStringSimple(params int[] regions);
}
