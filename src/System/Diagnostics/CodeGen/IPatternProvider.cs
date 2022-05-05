namespace System.Diagnostics.CodeGen;

/// <summary>
/// Indicates an attribute type that can control the customized patterns.
/// </summary>
internal interface IPatternProvider : ISourceGeneratorOptionProvider
{
	[DisallowNull]
	string? Pattern { get; init; }
}
