using Sudoku.Rendering.Nodes;

namespace Sudoku.Rendering;

/// <summary>
/// Provides with a data structure that displays a view for basic information.
/// </summary>
public sealed partial class View : HashSet<ViewNode>, ICloneable<View>
{
	/// <summary>
	/// Indicates the basic nodes that the current data type stores.
	/// </summary>
	public OfTypeEnumerator<BasicViewNode> BasicNodes => OfType<BasicViewNode>();

	/// <summary>
	/// Indicates icon nodes that the current data type stores.
	/// </summary>
	public OfTypeEnumerator<IconViewNode> FigureNodes => OfType<IconViewNode>();


	/// <summary>
	/// Adds a list of <see cref="ViewNode"/>s into the collection.
	/// </summary>
	/// <param name="nodes">A list of <see cref="ViewNode"/> instance.</param>
	public void AddRange(IEnumerable<ViewNode> nodes)
	{
		foreach (var node in nodes)
		{
			Add(node);
		}
	}

	/// <summary>
	/// Searches for an element that matches the conditions defined by the specified predicate,
	/// and returns the first occurrence within the entire <see cref="View"/>.
	/// </summary>
	/// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
	/// <returns>
	/// The first element that matches the conditions defined by the specified predicate, if found; otherwise, <see langword="null"/>.
	/// </returns>
	public ViewNode? Find(Predicate<ViewNode> match)
	{
		foreach (var element in this)
		{
			if (match(element))
			{
				return element;
			}
		}

		return null;
	}

	/// <summary>
	/// Filters the view nodes, only returns nodes of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of the node.</typeparam>
	/// <returns>The target collection of element type <typeparamref name="T"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public OfTypeEnumerator<T> OfType<T>() where T : ViewNode => new(this);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public View Clone() => Count == 0 ? [] : [.. from node in this select node.Clone()];
}
