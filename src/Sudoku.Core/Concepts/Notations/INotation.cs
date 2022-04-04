namespace Sudoku.Concepts.Notations;

/// <summary>
/// Defines a type that handles using the current notation.
/// </summary>
public interface INotationHandler
{
	/// <summary>
	/// Indicates the notation of the current provider type.
	/// </summary>
	Notation Notation { get; }
}
