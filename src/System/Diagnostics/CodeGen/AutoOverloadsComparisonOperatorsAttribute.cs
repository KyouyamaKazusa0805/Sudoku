namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attributes that allows user using it applying to a type
/// (especially for <see langword="class"/> or <see langword="struct"/>), indicating the overriden metadata
/// for <c><see langword="operator"/> &gt;</c> and <c><see langword="operator"/> &lt;</c>,
/// <c><see langword="operator"/> &gt;=</c> and <c><see langword="operator"/> &lt;=</c>,
/// and make source generator generates their own operators automatically.
/// </summary>
/// <remarks>
/// At default, the emitted generated operator overloading will use <see cref="IComparable{T}.CompareTo(T)"/> invocation.
/// </remarks>
/// <seealso cref="IComparable{T}.CompareTo(T)"/>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
public sealed class AutoOverloadsComparisonOperatorsAttribute :
	Attribute,
	IInModifierEmitter,
	INullableAnnotationEmitter
{
	/// <inheritdoc/>
	public bool EmitsInKeyword { get; init; } = false;

	/// <inheritdoc/>
	public bool WithNullableAnnotation { get; init; } = false;
}
