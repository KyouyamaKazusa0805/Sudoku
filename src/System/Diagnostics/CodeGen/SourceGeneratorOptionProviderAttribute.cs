namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute type that binds with a source generator, providing with the necessary and optional options
/// that is used or called by the source generator.
/// </summary>
public abstract class SourceGeneratorOptionProviderAttribute : Attribute, ISourceGeneratorOptionProvider
{
}
