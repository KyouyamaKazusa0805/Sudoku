namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attributes that allows user using it applying to a type
/// (especially for <see langword="class"/> or <see langword="struct"/>), indicating the overriden metadata
/// for <c><see langword="operator"/> ==</c> and <c><see langword="operator"/> !=</c>,
/// and make source generator generates their own operators automatically.
/// </summary>
/// <remarks>
/// At default, the emitted generated operator overloading will use <see cref="IEquatable{T}.Equals(T)"/> invocation.
/// </remarks>
/// <seealso cref="IEquatable{T}.Equals(T)"/>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class AutoOverloadsEqualityOperatorsAttribute :
	SourceGeneratorOptionProviderAttribute,
	IInModifierEmitter,
	INullableAnnotationEmitter
{
	/// <inheritdoc/>
	public bool EmitsInKeyword { get; init; } = false;

	/// <inheritdoc/>
	public bool WithNullableAnnotation { get; init; } = false;
}
