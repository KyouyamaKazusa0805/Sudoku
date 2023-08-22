namespace SudokuStudio.Collection;

/// <summary>
/// <para><inheritdoc cref="Stack{T}" path="/summary"/></para>
/// <para>Different with type <see cref="Stack{T}"/>, this type provides mechanism to trigger events on collection changed.</para>
/// </summary>
/// <typeparam name="T"><inheritdoc cref="Stack{T}" path="/typeparam[@name='T']"/></typeparam>
public sealed class ObservableStack<T> : Stack<T>, INotifyCollectionChanged
{
	/// <summary>
	/// Initializes an <see cref="ObservableStack{T}"/> instance.
	/// </summary>
	public ObservableStack() : base()
	{
	}

	/// <summary>
	/// Initializes an <see cref="ObservableStack{T}"/> instance via the specified list of elements.
	/// </summary>
	/// <param name="collection">The list of elements.</param>
	public ObservableStack(IEnumerable<T> collection) : base(collection)
	{
	}


	/// <summary>
	/// Defines an event that is triggered when the current collection is changed.
	/// </summary>
	public event ObservableStackChangedEventHandler<T>? Changed;

	/// <inheritdoc/>
	public event NotifyCollectionChangedEventHandler? CollectionChanged;


	/// <inheritdoc cref="Stack{T}.Push(T)"/>
	public new void Push(T element)
	{
		base.Push(element);

		Changed?.Invoke(this);
		CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, element));
	}

	/// <inheritdoc cref="Stack{T}.Pop"/>
	public new T Pop()
	{
		var result = base.Pop();

		Changed?.Invoke(this);
		CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, result));

		return result;
	}

	/// <inheritdoc cref="Stack{T}.Clear"/>
	public new void Clear()
	{
		base.Clear();

		Changed?.Invoke(this);
		CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Reset));
	}
}
