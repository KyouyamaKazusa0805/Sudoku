namespace Sudoku.CodeGenerating;

/// <summary>
/// Defines a preference route attribute that marks onto a preference item, to tell the source generator
/// that the preference item is bound with a <c>ToggleSwitch</c>, and creates a method
/// that delegated to the XAML file.
/// </summary>
/// <remarks>
/// <para>
/// For example, in the type <c>Preference</c> we can find many properties
/// that can both be settable and gettable. If I mark this attribute onto one property such as:
/// <code><![CDATA[
/// [ToggleSwitchRoute("ToggleSwitch_UseSizedFishName", "OptionItem_UseSizedFishName")]
/// public bool UseSizedFishName { get; set; } = false;
/// ]]></code>
/// Then we will get the delegated method:
/// <code><![CDATA[
/// private void ToggleSwitch_UseSizedFishName_Toggled(object sender, RoutedEventArgs e)
/// {
///     if (
///         (Sender: sender, PageIsInitialized: _pageIsInitialized) is (
///             Sender: ToggleSwitch { IsOn: var isOn },
///             PageIsInitialized: true
///         )
///     )
///     {
///         _boundSteps.Add(new(OptionItem_UseSizedFishName, () => _preference.UseSizedFishName = isOn));
///     }
/// }
/// ]]></code>
/// Where the field <c>_preference</c> and <c>_pageIsInitialized</c>
/// you can find them in the <c>SettingsPage</c>.
/// </para>
/// <para>
/// Please note that the method is a <see langword="private partial"/> method,
/// which means you must manually write the declaration into the type.
/// </para>
/// </remarks>
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
