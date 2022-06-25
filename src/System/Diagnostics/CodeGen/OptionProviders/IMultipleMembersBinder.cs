namespace System.Diagnostics.CodeGen.OptionProviders;

/// <summary>
/// Indicates an attribute type that is used for gathering a set of members.
/// </summary>
internal interface IMultipleMembersBinder : ISourceGeneratorOptionProvider
{
	/// <summary>
	/// Indicates the name of members that will be referenced.
	/// </summary>
	public abstract string[] MemberExpressions { get; }
}
