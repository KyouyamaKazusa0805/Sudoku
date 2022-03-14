using Sudoku.Diagnostics.CodeAnalysis;

namespace Sudoku.Test;

/// <summary>
/// Defines a node type that holds the maximum global ID value.
/// </summary>
/// <typeparam name="T">The type of the node.</typeparam>
internal interface IMaxGlobalId<[Self] T> where T : Node, IMaxGlobalId<T>
{
	/// <summary>
	/// Indicates the maximum global ID value that the current typed instance can be reached.
	/// </summary>
	static abstract int MaximumGlobalId { get; }
}
