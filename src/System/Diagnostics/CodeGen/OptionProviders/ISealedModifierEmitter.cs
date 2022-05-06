namespace System.Diagnostics.CodeGen.OptionProviders;

/// <summary>
/// Indicates an attribute type that can emit <see langword="sealed"/> keyword.
/// </summary>
internal interface ISealedModifierEmitter : ISourceGeneratorOptionProvider
{
	/// <summary>
	/// <para>
	/// Indicates whether the source generator will emit keyword <see langword="sealed"/> as modifier.
	/// </para>
	/// <para>The default value is <see langword="false"/>.</para>
	/// </summary>
	bool EmitsSealedKeyword { get; init; }
}
