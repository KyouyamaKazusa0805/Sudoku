namespace System.Diagnostics.CodeGen;

/// <summary>
/// Telling the source generators to generate extension deconstruction methods for a specified type.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
public sealed class AutoExtensionDeconstructionAttribute : Attribute
{
	/// <summary>
	/// Initializes an <see cref="AutoExtensionDeconstructionAttribute"/> instance.
	/// </summary>
	/// <param name="type">The type of the extension method to be generated.</param>
	/// <param name="memberExpressions">The member expressions to be generated.</param>
	/// <exception cref="ArgumentException"></exception>
	public AutoExtensionDeconstructionAttribute(Type type, params string[] memberExpressions)
		=> (Type, MemberExpressions) = (
			type.IsAssignableTo(typeof(Delegate)) || type.IsAssignableTo(typeof(Enum))
				? throw new ArgumentException("The type cannot be a delegate or enumeration.", nameof(type))
				: type,
			memberExpressions.Length == 0
				? throw new ArgumentException("The argument cannot be empty.", nameof(memberExpressions))
				: memberExpressions
		);


	/// <summary>
	/// Indicates whether the source generators also emit for <see langword="in"/>
	/// and <see langword="scoped"/> keywords if available on <see langword="struct"/> types.
	/// </summary>
	/// <remarks>The default value is <see langword="false"/>.</remarks>
	public bool EmitsInKeyword { get; init; } = false;

	/// <summary>
	/// The namespace that the containing type of extension methods exists.
	/// </summary>
	/// <remarks>The default value is <see langword="null"/>.</remarks>
	public string? Namespace { get; init; } = null;

	/// <summary>
	/// The member expressions to be converted.
	/// </summary>
	public string[] MemberExpressions { get; }

	/// <summary>
	/// The type to be generated.
	/// </summary>
	public Type Type { get; }
}
