namespace System.Linq.Enumerators;

/// <summary>
/// Represents an enumerator type that can iterate on each function or action on a complex delegate object
/// of type <typeparamref name="TDelegate"/>.
/// </summary>
/// <typeparam name="TDelegate">The type of each function or action.</typeparam>
/// <param name="value">The complex delegate object to be iterated.</param>
[StructLayout(LayoutKind.Auto)]
[TypeImpl(
	TypeImplFlag.AllObjectMethods | TypeImplFlag.Disposable,
	OtherModifiersOnDisposableDispose = "readonly",
	ExplicitlyImplsDisposable = true)]
public ref partial struct DelegateEnumerator<TDelegate>([Property] TDelegate? value) : IEnumerator<TDelegate>
	where TDelegate : Delegate
{
	/// <summary>
	/// Indicates the backing enumerator.
	/// </summary>
	private Delegate.InvocationListEnumerator<TDelegate> _enumerator = Delegate.EnumerateInvocationList(value);


	/// <inheritdoc cref="IEnumerator{T}.Current"/>
	public readonly TDelegate Current => _enumerator.Current;

	/// <inheritdoc/>
	readonly object IEnumerator.Current => Current;


	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public bool MoveNext() => _enumerator.MoveNext();

	/// <inheritdoc/>
	[DoesNotReturn]
	readonly void IEnumerator.Reset() => throw new NotImplementedException();
}
