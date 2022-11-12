namespace Sudoku.Presentation;

/// <summary>
/// Defines a view node segment.
/// </summary>
public readonly struct ViewNodeSegment
{
	/// <summary>
	/// Initializes a <see cref="ViewNodeSegment"/> instance via view nodes.
	/// </summary>
	/// <param name="viewNodes">View nodes.</param>
	/// <param name="count">The number of elements.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ViewNodeSegment(object viewNodes, int count) => (ActualValue, CollectionValuesCount) = (viewNodes, count);


	/// <summary>
	/// The actual value. The compatible types of this property can be <see cref="ViewNode"/>, <see cref="ViewNode"/>[]
	/// and <see cref="List{T}"/> of <see cref="ViewNode"/>.
	/// </summary>
	public object ActualValue { get; }

	/// <summary>
	/// Indicates the number of elements stored in property <see cref="ActualValue"/> of this instance.
	/// </summary>
	internal int CollectionValuesCount { get; }


	/// <summary>
	/// Convert the current instance into an array, no matter what the inner type is. This method will not throw any exceptions;
	/// it will always create an array and copy values into it if the inner field is not an array.
	/// </summary>
	/// <returns>An array of <see cref="ViewNode"/>s.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ViewNode[] ToArray() => ActualValue switch { ViewNode v => new[] { v }, ViewNode[] v => v, _ => ((List<ViewNode>)ActualValue).ToArray() };


	/// <summary>
	/// Implicit cast from <see cref="ViewNode"/> to <see cref="ViewNodeSegment"/>.
	/// </summary>
	/// <param name="viewNode">View node.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator ViewNodeSegment(ViewNode viewNode) => new(viewNode, 1);

	/// <summary>
	/// Implicit cast from <see cref="ViewNode"/>[] to <see cref="ViewNodeSegment"/>.
	/// </summary>
	/// <param name="viewNodes">View nodes.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator ViewNodeSegment(ViewNode[] viewNodes) => new(viewNodes, viewNodes.Length);

	/// <summary>
	/// Implicit cast from <see cref="List{T}"/> of <see cref="ViewNode"/> to <see cref="ViewNodeSegment"/>.
	/// </summary>
	/// <param name="viewNodes">View nodes.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator ViewNodeSegment(List<ViewNode> viewNodes) => new(viewNodes, viewNodes.Count);
}
