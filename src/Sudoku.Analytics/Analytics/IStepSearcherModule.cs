namespace Sudoku.Analytics;

/// <summary>
/// Represents for a step searcher module. The module is used for extracting logic, without defining base and derived type hierarchy.
/// </summary>
/// <typeparam name="TSelf">The type of the module.</typeparam>
public interface IStepSearcherModule<in TSelf> where TSelf : IStepSearcherModule<TSelf>
{
	/// <summary>
	/// Indicates the supported types.
	/// </summary>
	public static abstract Type[] SupportedTypes { get; }
}
