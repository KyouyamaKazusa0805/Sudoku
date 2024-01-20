namespace System.Numerics;

/// <summary>
/// Represents an enumerator that iterates an <see cref="nint"/> or <see cref="nuint"/> value.
/// </summary>
/// <param name="value">The value to be iterated.</param>
[StructLayout(LayoutKind.Auto)]
public ref partial struct NativeIntEnumerator([RecordParameter(DataMemberKinds.Field, IsImplicitlyReadOnly = false)] nuint value)
{
	/// <inheritdoc cref="IEnumerator{T}.Current"/>
	public int Current { get; private set; } = -1;


	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public unsafe bool MoveNext()
	{
		while (++Current < sizeof(nuint) << 3)
		{
			if ((_value >> Current & 1) != 0)
			{
				return true;
			}
		}

		return false;
	}
}
