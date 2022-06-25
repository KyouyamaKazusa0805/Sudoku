namespace System.Diagnostics.CodeGen.OptionProviders;

/// <summary>
/// Indicates an attribute type that binds with type.
/// </summary>
internal interface ITypeBinder : ISourceGeneratorOptionProvider
{
	/// <summary>
	/// Indicates the type that the attribute used and bound.
	/// </summary>
	public abstract Type Type { get; }
}
