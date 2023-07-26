namespace Sudoku.Rendering;

/// <summary>
/// Provides with a data structure that displays a view for basic information.
/// </summary>
public sealed partial class View : HashSet<ViewNode>, ICloneable<View>
{
	/// <summary>
	/// Creates an empty <see cref="View"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public View() : base()
	{
	}

	/// <summary>
	/// Initializes a <see cref="View"/> instance via the specified list as the raw value.
	/// </summary>
	/// <param name="nodes">The list as the raw value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private View(HashSet<ViewNode> nodes) : base(nodes)
	{
	}


	/// <summary>
	/// Indicates the basic nodes that the current data type stores.
	/// </summary>
	public OfTypeEnumerator<BasicViewNode> BasicNodes => OfType<BasicViewNode>();

	/// <summary>
	/// Indicates icon nodes that the current data type stores.
	/// </summary>
	public OfTypeEnumerator<IconViewNode> FigureNodes => OfType<IconViewNode>();


	/// <summary>
	/// Appends a new <see cref="ViewNode"/> into the current collection if the specified argument isn't <see langword="null"/>.
	/// </summary>
	/// <param name="node">A possible node to be appended. If the value is <see langword="null"/>, it will be ignored.</param>
	/// <remarks>
	/// The reason why the parameter <paramref name="node"/> is nullable is that C# 12 feature "Collection Literals" use this method
	/// to append elements.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public new void Add(ViewNode? node)
	{
		if (node is not null)
		{
			base.Add(node);
		}
	}

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
	public View Clone() => Count == 0 ? [] : new([.. from node in this select node.Clone()]);
}
