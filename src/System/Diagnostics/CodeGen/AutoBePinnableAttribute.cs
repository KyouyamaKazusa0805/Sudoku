namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that can be applied to a type (especially for
/// <see langword="struct"/> or <see langword="class"/>), indicating the source generator
/// will automatically generate <c>GetPinnableReference</c> method.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
public sealed class AutoBePinnableAttribute : Attribute, ITypeBinder, IPatternProvider
{
	/// <summary>
	/// Initializes an <see cref="AutoBePinnableAttribute"/> instance via the specified returning type,
	/// and the pattern.
	/// </summary>
	/// <param name="returnType">The type of the return value.</param>
	/// <param name="pattern">The pattern.</param>
	public AutoBePinnableAttribute(Type returnType, string pattern) => (Type, Pattern) = (returnType, pattern);


	/// <summary>
	/// Indicates whether the return value is <see langword="ref readonly"/>. The default value is <see langword="true"/>.
	/// </summary>
	public bool ReturnsReadOnlyReference { get; init; } = true;

	/// <summary>
	/// Indicates the pattern.
	/// </summary>
	public string? Pattern { get; }

	/// <inheritdoc/>
	public Type Type { get; }

	/// <inheritdoc/>
	[DisallowNull]
	string? IPatternProvider.Pattern
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Pattern;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => throw new NotSupportedException();
	}
}
