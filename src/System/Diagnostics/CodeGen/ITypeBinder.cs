namespace System.Diagnostics.CodeGen;

/// <summary>
/// Indicates an attribute type that binds with type.
/// </summary>
internal interface ITypeBinder : ISourceGeneratorOptionProvider
{
	Type Type { get; }
}
