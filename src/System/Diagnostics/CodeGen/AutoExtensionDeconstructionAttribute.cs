namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that is used for controlling the source generation on extension deconstruction methods.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
public sealed class AutoExtensionDeconstructionAttribute : Attribute
{
	/// <summary>
	/// Initializes an <see cref="AutoExtensionDeconstructionAttribute"/> instance via the specified type,
	/// and the member expression.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="memberExpression">The member expression.</param>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="type"/> is not a valid type (<see langword="class"/>,
	/// <see langword="struct"/> or <see langword="interface"/>),
	/// or the argument <paramref name="memberExpression"/> is empty.
	/// </exception>
	public AutoExtensionDeconstructionAttribute(Type type, params string[] memberExpression)
		=> (Type, MemberExpression) = (
			type.IsAssignableTo(typeof(Delegate)) || type.IsAssignableTo(typeof(Enum))
				? throw new ArgumentException("The type cannot be a delegate or enumeration.", nameof(type))
				: type,
			memberExpression.Length == 0
				? throw new ArgumentException("The argument cannot be empty.", nameof(memberExpression))
				: memberExpression
		);

	/// <summary>
	/// <para>
	/// Indicates whether the source generator will emit keyword <see langword="in"/> as modifier
	/// for the only <see langword="this"/> argument.
	/// </para>
	/// <para>The default value is <see langword="false"/>.</para>
	/// </summary>
	public bool EmitsInKeyword { get; init; } = false;

	/// <summary>
	/// <para>Indicates the namespace the source generator emits.</para>
	/// <para>The default value is <see langword="null"/>.</para>
	/// </summary>
	public string? Namespace { get; init; } = null;

	/// <summary>
	/// Indicates the member names whose corresponding members will be able to be deconstructed.
	/// </summary>
	public string[] MemberExpression { get; }

	/// <summary>
	/// Indicates the desired type on which the source generator emits extension methods.
	/// </summary>
	public Type Type { get; }
}
