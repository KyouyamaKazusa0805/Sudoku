using System.Collections;
using System.Runtime.InteropServices;
using System.SourceGeneration;

namespace System.Runtime.CompilerServices;

/// <summary>
/// Represents for an enumerator that iterates on each elements stored in a <see cref="ITuple"/>.
/// </summary>
/// <param name="tuple">A tuple instance.</param>
[StructLayout(LayoutKind.Auto)]
public ref partial struct TupleEnumerator([DataMember(MemberKinds.Field)] ITuple tuple)
{
	/// <summary>
	/// The current index.
	/// </summary>
	private int _index = -1;


	/// <inheritdoc cref="IEnumerator.Current"/>
	public readonly object? Current => _tuple[_index];


	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public bool MoveNext() => ++_index < _tuple.Length;
}
