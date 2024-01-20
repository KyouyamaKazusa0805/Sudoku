namespace System.Numerics;

/// <summary>
/// Represents an enumerator that iterates an <see cref="int"/> or <see cref="uint"/> value.
/// </summary>
/// <param name="value">The value to be iterated.</param>
[StructLayout(LayoutKind.Auto)]
public ref partial struct Int32Enumerator([RecordParameter(DataMemberKinds.Field, IsImplicitlyReadOnly = false)] uint value)
{
	/// <inheritdoc cref="IEnumerator{T}.Current"/>
	public int Current { get; private set; } = -1;


	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public bool MoveNext()
	{
		while (++Current < 32)
		{
			if ((_value >> Current & 1) != 0)
			{
				return true;
			}
		}

		return false;
	}
}
