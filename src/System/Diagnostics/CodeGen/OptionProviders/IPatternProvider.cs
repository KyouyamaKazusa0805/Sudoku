namespace System.Diagnostics.CodeGen.OptionProviders;

/// <summary>
/// Indicates an attribute type that can control the customized patterns.
/// </summary>
internal interface IPatternProvider : ISourceGeneratorOptionProvider
{
	[DisallowNull]
	string? Pattern { get; init; }
}
