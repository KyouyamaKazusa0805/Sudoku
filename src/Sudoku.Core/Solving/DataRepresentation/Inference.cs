namespace Sudoku.Concepts;

/// <summary>
/// Defines an inference.
/// </summary>
[JsonConverter(typeof(InferenceJsonConverter))]
[EnumSwitchExpressionRoot("GetIdentifier", MethodDescription = "Gets the identifier of the inference.", ThisParameterDescription = "The inference.", ReturnValueDescription = "The identifier value.")]
public enum Inference : byte
{
	/// <summary>
	/// Indicates the inference is strong inference.
	/// </summary>
	[EnumSwitchExpressionArm("GetIdentifier", " == ")]
	Strong,

	/// <summary>
	/// Indicates the inference is weak inference.
	/// </summary>
	[EnumSwitchExpressionArm("GetIdentifier", " -- ")]
	Weak,

	/// <summary>
	/// Indicates the inference is strong inference that is generalized.
	/// </summary>
	[EnumSwitchExpressionArm("GetIdentifier", " =~ ")]
	StrongGeneralized,

	/// <summary>
	/// Indicates the inference is weak inference that is generalized.
	/// </summary>
	[EnumSwitchExpressionArm("GetIdentifier", " -~ ")]
	WeakGeneralized,

	/// <summary>
	/// Indicates the inference is conjugate pair.
	/// </summary>
	[EnumSwitchExpressionArm("GetIdentifier", " == ")]
	ConjuagtePair,

	/// <summary>
	/// Indicates the inference is the default case that doesn't belong to above.
	/// </summary>
	[EnumSwitchExpressionArm("GetIdentifier", " -- ")]
	Default
}
