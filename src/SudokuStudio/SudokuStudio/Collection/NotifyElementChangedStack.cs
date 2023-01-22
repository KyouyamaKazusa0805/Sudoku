namespace SudokuStudio.Collection;

/// <summary>
/// <para><inheritdoc cref="Stack{T}" path="/summary"/></para>
/// <para>Different with type <see cref="Stack{T}"/>, this type provides mechanism to trigger events on collection changed.</para>
/// </summary>
/// <typeparam name="T"><inheritdoc cref="Stack{T}" path="/typeparam[@name='T']"/></typeparam>
public sealed class NotifyElementChangedStack<T> : Stack<T>
{
	/// <summary>
	/// Initializes an <see cref="NotifyElementChangedStack{T}"/> instance.
	/// </summary>
	public NotifyElementChangedStack() : base()
	{
	}

	/// <summary>
	/// Initializes an <see cref="NotifyElementChangedStack{T}"/> instance via the specified list of elements.
	/// </summary>
	/// <param name="collection">The list of elements.</param>
	public NotifyElementChangedStack(IEnumerable<T> collection) : base(collection)
	{
	}


	/// <summary>
	/// Defines an event that is triggered when the current collection is changed.
	/// </summary>
	public event ObservableStackChangedEventHandler<T>? Changed;


	/// <inheritdoc cref="Stack{T}.Push(T)"/>
	public new void Push(T element)
	{
		base.Push(element);

		Changed?.Invoke(this);
	}

	/// <inheritdoc cref="Stack{T}.Pop"/>
	public new T Pop()
	{
		var result = base.Pop();

		Changed?.Invoke(this);

		return result;
	}

	/// <inheritdoc cref="Stack{T}.Clear"/>
	public new void Clear()
	{
		base.Clear();

		Changed?.Invoke(this);
	}
}
