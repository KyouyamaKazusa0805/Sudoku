using static System.AttributeTargets;

namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that is used for controlling the source generation on deconstruction methods.
/// </summary>
[AttributeUsage(Class | Struct | Interface, AllowMultiple = true, Inherited = false)]
public sealed class AutoDeconstructionAttribute : Attribute
{
	/// <summary>
	/// Initializes an <see cref="AutoDeconstructionAttribute"/> instance
	/// via the specified member expressions you want to deconstruct.
	/// </summary>
	/// <param name="memberExpression">The name of the members you want to deconstruct.</param>
	/// <exception cref="ArgumentException">Throws when the argument is empty.</exception>
	public AutoDeconstructionAttribute(params string[] memberExpression)
		=> MemberExpression = memberExpression.Length == 0
			? throw new ArgumentException("You must set at least one instance to be deconstructed.")
			: memberExpression;


	/// <summary>
	/// Indicates the member names whose corresponding members will be able to be deconstructed.
	/// </summary>
	public string[] MemberExpression { get; }
}
