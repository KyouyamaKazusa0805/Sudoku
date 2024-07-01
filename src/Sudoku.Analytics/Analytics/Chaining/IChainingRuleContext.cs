namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents a context type consumed by <see cref="ChainingRule"/>.
/// </summary>
/// <seealso cref="ChainingRule"/>
internal interface IChainingRuleContext
{
	/// <summary>
	/// Indicates the grid to be used.
	/// </summary>
	public ref readonly Grid Grid { get; }
}
