namespace Sudoku.Runtime.AnalysisServices;

/// <summary>
/// Defines an attribute that modifies a type of a <see langword="class"/> or a <see langword="struct"/>,
/// indicating the type having been modified this attribute is reserved for the developers used for studying.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class AlgorithmAttribute : Attribute
{
	/// <summary>
	/// Initializes an <see cref="AlgorithmAttribute"/> instance via the specified algorithm name.
	/// </summary>
	/// <param name="algorithmName">The name of the algorithm.</param>
	public AlgorithmAttribute(string algorithmName) => AlgorithmName = algorithmName;


	/// <summary>
	/// Indicates the name of the algorithm.
	/// </summary>
	public string AlgorithmName { get; }

	/// <summary>
	/// Indicates the URI link for the introduction about the algorithm.
	/// </summary>
	public string? UriLink { get; init; }
}
