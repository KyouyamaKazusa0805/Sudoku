namespace System.Diagnostics.CodeGen.OptionProviders;

/// <summary>
/// Indicates an attribute type that can emit customized namespaces.
/// </summary>
internal interface ICustomizedNamespaceEmitter : ISourceGeneratorOptionProvider
{
	/// <summary>
	/// <para>Indicates the namespace the source generator emits.</para>
	/// <para>The default value is <see langword="null"/>.</para>
	/// </summary>
	string? Namespace { get; init; }
}
