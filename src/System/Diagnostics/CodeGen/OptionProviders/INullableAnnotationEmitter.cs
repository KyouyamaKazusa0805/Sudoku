namespace System.Diagnostics.CodeGen.OptionProviders;

/// <summary>
/// Indicates an attribute type that can emit nullable annotation <c>?</c>.
/// </summary>
internal interface INullableAnnotationEmitter : ISourceGeneratorOptionProvider
{
	/// <summary>
	/// Indicates whether the source generator will emit nullable annotation <c>?</c> onto the arguments.
	/// The default value is <see langword="false"/>.
	/// </summary>
	bool WithNullableAnnotation { get; init; }
}
