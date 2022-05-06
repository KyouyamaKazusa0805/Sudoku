namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that is used for controlling the source generation on deconstruction methods.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = true)]
public sealed class AutoDeconstructionAttribute : SourceGeneratorOptionProviderAttribute, IMultipleMembersBinder
{
	/// <summary>
	/// Initializes an <see cref="AutoDeconstructionAttribute"/> instance
	/// via the specified member expressions you want to deconstruct.
	/// </summary>
	/// <param name="memberExpressions">The name of the members you want to deconstruct.</param>
	/// <exception cref="ArgumentException">Throws when the argument is empty.</exception>
	public AutoDeconstructionAttribute(params string[] memberExpressions)
		=> MemberExpressions = memberExpressions.Length == 0
			? throw new ArgumentException("You must set at least one instance to be deconstructed.")
			: memberExpressions;


	/// <inheritdoc/>
	public string[] MemberExpressions { get; }
}
