namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Indicates the source generator should generate its backing field without compiler controlling.
/// </summary>
/// <remarks>
/// You can use this attribute to create a field:
/// <code><![CDATA[
/// [ImplicitField]
/// public int Property
/// {
///	    get => _property;
///	    set => _property ??= value;
/// }
/// ]]></code>
/// You may not write code to create field <c>_property</c>. This attribute will make source generators generate it.
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ImplicitFieldAttribute : Attribute
{
	/// <summary>
	/// Indicates whether the generated field automatically appends a <see langword="readonly"/> modifier.
	/// The value is <see langword="true"/> by default.
	/// </summary>
	public bool RequiredReadOnlyModifier { get; init; } = true;
}
