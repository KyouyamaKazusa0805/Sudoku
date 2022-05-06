namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that is used for controlling the source generation on automatically implementing
/// <see cref="IComparable{T}"/>.
/// </summary>
/// <seealso cref="IComparable{T}"/>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class AutoImplementsComparableAttribute :
	SourceGeneratorOptionProviderAttribute,
	IInterfaceImplementingCaseController,
	ISingleMemberBinder
{
	/// <summary>
	/// Initializes an <see cref="AutoImplementsComparableAttribute"/> instance via the member name.
	/// </summary>
	/// <param name="memberName">The member name.</param>
	public AutoImplementsComparableAttribute(string? memberName = null) => MemberName = memberName;


	/// <inheritdoc/>
	public bool UseExplicitImplementation { get; init; } = false;

	/// <inheritdoc/>
	public string? MemberName { get; }
}
