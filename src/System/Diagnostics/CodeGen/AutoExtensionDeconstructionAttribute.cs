namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that is used for controlling the source generation on extension deconstruction methods.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
public sealed class AutoExtensionDeconstructionAttribute :
	Attribute,
	IMultipleMembersBinder,
	IInModifierEmitter,
	ICustomizedNamespaceEmitter,
	ITypeBinder
{
	/// <summary>
	/// Initializes an <see cref="AutoExtensionDeconstructionAttribute"/> instance via the specified type,
	/// and the member expression.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="memberExpressions">The member expression.</param>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="type"/> is not a valid type (<see langword="class"/>,
	/// <see langword="struct"/> or <see langword="interface"/>),
	/// or the argument <paramref name="memberExpressions"/> is empty.
	/// </exception>
	public AutoExtensionDeconstructionAttribute(Type type, params string[] memberExpressions)
		=> (Type, MemberExpressions) = (
			type.IsAssignableTo(typeof(Delegate)) || type.IsAssignableTo(typeof(Enum))
				? throw new ArgumentException("The type cannot be a delegate or enumeration.", nameof(type))
				: type,
			memberExpressions.Length == 0
				? throw new ArgumentException("The argument cannot be empty.", nameof(memberExpressions))
				: memberExpressions
		);

	/// <inheritdoc/>
	public bool EmitsInKeyword { get; init; } = false;

	/// <inheritdoc/>
	public string? Namespace { get; init; } = null;

	/// <inheritdoc/>
	public string[] MemberExpressions { get; }

	/// <inheritdoc/>
	public Type Type { get; }
}
