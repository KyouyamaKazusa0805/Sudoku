namespace System.Diagnostics.CodeGen;

/// <summary>
/// Represents an attribute that limits a <see langword="struct"/> type cannot create an instance by
/// the parameterless constructor.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class DisableParameterlessConstructorAttribute : Attribute
{
	/// <summary>
	/// Indicates the data member name you want to allow the user using.
	/// </summary>
	public string? MemberName { get; init; }

	/// <summary>
	/// Indicates the message you want to tell the user.
	/// </summary>
	public string? Message { get; init; }
}
