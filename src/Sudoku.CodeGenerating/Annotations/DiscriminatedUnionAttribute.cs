namespace Sudoku.CodeGenerating;

/// <summary>
/// Indicates the type is a discriminated union.
/// </summary>
/// <remarks>
/// A <b>discriminated union</b> is a type that is a same implementation
/// for the concept <see langword="enum class"/> in Java.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class DiscriminatedUnionAttribute : Attribute
{
	/// <summary>
	/// <para>
	/// A <see cref="bool"/> value that indicates whether an <see cref="InvalidOperationException"/> throws
	/// when the value is out of range.
	/// </para>
	/// <para>The default value is <c><see langword="false"/>.</c></para>
	/// </summary>
	public bool ExceptionThrowsWhenOutOfRange { get; init; }
}
