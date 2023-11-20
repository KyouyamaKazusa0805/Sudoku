using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.SourceGeneration;

namespace Sudoku.Rendering;

/// <summary>
/// Represents an enumerator that iterates for <typeparamref name="T"/>-typed instances.
/// </summary>
/// <typeparam name="T">The type of the element node.</typeparam>
/// <param name="enumerator">The enumerator.</param>
[StructLayout(LayoutKind.Auto)]
[Equals]
[GetHashCode]
[ToString]
public ref partial struct ViewNodeIterator<T>([Data(DataMemberKinds.Field, IsImplicitlyReadOnly = false)] View.Enumerator enumerator)
	where T : ViewNode
{
	/// <inheritdoc cref="IEnumerator{T}.Current"/>
	public T Current { get; private set; }


	/// <summary>
	/// Creates an <see cref="ViewNodeIterator{T}"/> instance.
	/// </summary>
	/// <returns>An <see cref="ViewNodeIterator{T}"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly ViewNodeIterator<T> GetEnumerator() => this;

	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public bool MoveNext()
	{
		while (_enumerator.MoveNext())
		{
			if (_enumerator.Current is T current)
			{
				Current = current;
				return true;
			}
		}

		return false;
	}
}
