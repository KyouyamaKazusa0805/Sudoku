namespace Sudoku.Analytics.Configuration;

/// <summary>
/// Represents an option type.
/// </summary>
/// <typeparam name="TSelf">The type of itself.</typeparam>
public interface IStepSearcherOptions<TSelf> : IEquatable<TSelf> where TSelf : class?, IEquatable<TSelf>?, IStepSearcherOptions<TSelf>?
{
	/// <summary>
	/// Represents a default instance.
	/// </summary>
	public static abstract TSelf Default { get; }
}
