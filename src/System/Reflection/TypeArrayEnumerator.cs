#if NATIVE_AOT
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.SourceGeneration;

namespace System.Reflection;

/// <summary>
/// Represents a <see cref="Type"/> array enumerator. This type solves for trimming rule.
/// </summary>
/// <param name="types">The types.</param>
/// <seealso cref="Type"/>
[StructLayout(LayoutKind.Auto)]
public ref partial struct TypeArrayEnumerator([DataMember(MemberKinds.Field)] Type[] types)
{
	/// <summary>
	/// Indicates the index.
	/// </summary>
	private int _index = -1;


	/// <inheritdoc cref="IEnumerator{T}.Current"/>
	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
	[SuppressMessage("Trimming", "IL2063:The return value of method has a DynamicallyAccessedMembersAttribute, but the value returned from the method can not be statically analyzed.", Justification = "<Pending>")]
	public readonly Type Current => _types[_index];


	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public bool MoveNext() => ++_index <= _types.Length;


	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	public readonly TypeArrayEnumerator GetEnumerator() => this;
}
#endif