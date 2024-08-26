namespace Sudoku.Runtime;

/// <summary>
/// Represents a type that is a context.
/// </summary>
public interface IContext
{
	/// <summary>
	/// Indicates the grid to be used.
	/// </summary>
	public abstract ref readonly Grid Grid { get; }
}
