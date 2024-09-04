namespace System;

/// <summary>
/// Defines an enumerator that iterates the possible fields of an enumeration type.
/// </summary>
/// <typeparam name="T">
/// The type of the enumeration type, that is marked the attribute <see cref="FlagsAttribute"/>.
/// </typeparam>
/// <param name="baseField">Indicates the base field.</param>
[StructLayout(LayoutKind.Auto)]
public ref partial struct EnumFlagsEnumerator<T>([PrimaryConstructorParameter(MemberKinds.Field)] T baseField) : IEnumerator<T>
	where T : unmanaged, Enum
{
	/// <summary>
	/// Indicates the fields of the type to iterate.
	/// </summary>
	private readonly T[] _fields = Enum.GetValues<T>();

	/// <summary>
	/// Indicates the current index being iterated.
	/// </summary>
	private int _index = -1;


	/// <inheritdoc cref="IEnumerator.Current"/>
	public T Current { get; private set; } = default;

	/// <inheritdoc/>
	readonly object IEnumerator.Current => Current;


	/// <summary>
	/// Indicates the size of <typeparamref name="T"/>.
	/// </summary>
	private static unsafe int SizeOfT => sizeof(T);


	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public bool MoveNext()
	{
		for (var index = _index + 1; index < _fields.Length; index++)
		{
			var field = _fields[index];
			switch (SizeOfT)
			{
				case 1 or 2 or 4 when int.IsPow2(Unsafe.As<T, int>(ref field)) && _baseField.HasFlag(field):
				{
					Current = _fields[_index = index];
					return true;
				}
				case 8 when long.IsPow2(Unsafe.As<T, long>(ref field)) && _baseField.HasFlag(field):
				{
					Current = _fields[_index = index];
					return true;
				}
			}
		}
		return false;
	}

	/// <inheritdoc/>
	[DoesNotReturn]
	readonly void IEnumerator.Reset() => throw new NotImplementedException();

	/// <inheritdoc/>
	readonly void IDisposable.Dispose() { }
}
