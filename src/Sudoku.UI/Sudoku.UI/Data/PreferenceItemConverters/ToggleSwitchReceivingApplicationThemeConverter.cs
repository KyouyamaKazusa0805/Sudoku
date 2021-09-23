namespace Sudoku.UI.Data.PreferenceItemConverters;

/// <summary>
/// Defines a converter that allows the value converting
/// from a <see cref="ApplicationTheme"/> to a <see cref="bool"/>.
/// </summary>
[PreferenceItemConverter<ApplicationTheme, bool>]
public sealed class ToggleSwitchReceivingApplicationThemeConverter : PreferenceItemConverter<ToggleSwitch>
{
	/// <inheritdoc/>
	public override bool Bind(object? value, ToggleSwitch control)
	{
		if (value is ApplicationTheme v)
		{
			control.IsOn = v == ApplicationTheme.Dark;
			return true;
		}

		return false;
	}
}
