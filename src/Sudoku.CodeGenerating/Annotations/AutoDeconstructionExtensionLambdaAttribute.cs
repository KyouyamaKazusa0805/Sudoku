namespace Sudoku.CodeGenerating;

/// <summary>
/// Mark this attribute onto a type, to tell the source generator that the source generator will
/// generates the extension deconstruction methods, with expressions.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
public sealed class AutoDeconstructionExtensionLambdaAttribute : Attribute
{
	/// <summary>
	/// Initializes an <see cref="AutoDeconstructLambdaAttribute"/> instance via the member names.
	/// </summary>
	/// <param name="memberNames">The member names.</param>
	public AutoDeconstructionExtensionLambdaAttribute(params string[] memberNames) => MemberNames = memberNames;


	/// <summary>
	/// Indicates the member names to deconstruct.
	/// </summary>
	public string[] MemberNames { get; }
}
