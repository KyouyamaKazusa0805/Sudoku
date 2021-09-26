namespace Sudoku.CodeGenerating;

/// <summary>
/// Defines a preference route attribute that marks onto a preference item, to tell the source generator
/// that the preference item is bound with a <c>ToggleSwitch</c>, and creates a method
/// that delegated to the XAML file.
/// </summary>
public sealed class ToggleSwitchRouteAttribute : PreferenceRouteAttribute
{
	/// <summary>
	/// Initializes a <see cref="ToggleSwitchRouteAttribute"/> instance with the specified control name.
	/// </summary>
	/// <param name="controlName">The bound control name.</param>
	/// <param name="effectControlName">Indicates the effect control name.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ToggleSwitchRouteAttribute(string controlName, string effectControlName)
	{
		ControlName = controlName;
		EffectControlName = effectControlName;
	}


	/// <summary>
	/// Indicates the bound control name.
	/// </summary>
	public string ControlName { get; }

	/// <summary>
	/// Indicates the effect control name.
	/// </summary>
	public string EffectControlName { get; }

	/// <summary>
	/// Indicates the method name that executes the code, to assign the result.
	/// </summary>
#if NETSTANDARD2_1_OR_GREATER
	[DisallowNull]
#endif
	public string? PreferenceSetterMethodName { get; init; }
}
