namespace System.Diagnostics.CodeGen;

/// <summary>
/// Indicates an attribute type that binds with type.
/// </summary>
internal interface ITypeBinder : ISourceGeneratorOptionProvider
{
	/// <summary>
	/// Indicates the type that the attribute used and bound.
	/// </summary>
	Type Type { get; }
}
