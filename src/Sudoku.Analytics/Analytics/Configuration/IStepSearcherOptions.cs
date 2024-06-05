namespace Sudoku.Analytics.Configuration;

/// <summary>
/// Represents an option type that will be consumed by <see cref="Analyzer"/>.
/// </summary>
/// <typeparam name="TSelf">The type of itself.</typeparam>
/// <seealso cref="Analyzer"/>
public interface IStepSearcherOptions<TSelf> : IEquatable<TSelf> where TSelf : class?, IEquatable<TSelf>?, IStepSearcherOptions<TSelf>?
{
	/// <summary>
	/// Represents a default instance.
	/// </summary>
	public static abstract TSelf Default { get; }
}
