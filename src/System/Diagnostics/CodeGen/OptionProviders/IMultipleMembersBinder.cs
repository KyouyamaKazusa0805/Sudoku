namespace System.Diagnostics.CodeGen;

/// <summary>
/// Indicates an attribute type that is used for gathering a set of members.
/// </summary>
internal interface IMultipleMembersBinder : ISourceGeneratorOptionProvider
{
	/// <summary>
	/// Indicates the name of members that will be referenced.
	/// </summary>
	string[] MemberExpressions { get; }
}
