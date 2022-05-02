namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that is used for controlling the source generation on automatically implementing
/// <see cref="IComparable{T}"/>.
/// </summary>
/// <seealso cref="IComparable{T}"/>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
public sealed class AutoImplementsComparableAttribute : Attribute
{
	/// <summary>
	/// Initializes an <see cref="AutoImplementsComparableAttribute"/> instance via the member name.
	/// </summary>
	/// <param name="memberName">The member name.</param>
	public AutoImplementsComparableAttribute(string? memberName = null) => MemberName = memberName;


	/// <summary>
	/// Indiactes whether the source generator will emit explicit interface implementation instead
	/// of implicit one. The default value is <see langword="false"/>.
	/// </summary>
	public bool UseExplicitImplementation { get; init; } = false;

	/// <summary>
	/// Indicates the member name to compare.
	/// </summary>
	public string? MemberName { get; }
}
