namespace System.Diagnostics.CodeGen.OptionProviders;

/// <summary>
/// Indicates an attribute type that can emit <see langword="in"/> keyword.
/// </summary>
internal interface IInModifierEmitter : ISourceGeneratorOptionProvider
{
	/// <summary>
	/// <para>
	/// Indicates whether the source generator will emit keyword <see langword="in"/> as modifier.
	/// </para>
	/// <para>The default value is <see langword="false"/>.</para>
	/// </summary>
	bool EmitsInKeyword { get; init; }
}
