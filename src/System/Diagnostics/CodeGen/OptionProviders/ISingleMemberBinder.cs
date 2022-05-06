namespace System.Diagnostics.CodeGen.OptionProviders;

/// <summary>
/// Indicates an attribute type that binds with a single member, referenced by its name.
/// </summary>
internal interface ISingleMemberBinder : ISourceGeneratorOptionProvider
{
	/// <summary>
	/// Indicates the member name to be referenced.
	/// </summary>
	string? MemberName { get; }
}
