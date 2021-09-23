namespace Sudoku.UI.Data.PreferenceItemConverters;

/// <summary>
/// Defines a converter that allows the value converting from a <see cref="bool"/> to a <see cref="bool"/>.
/// </summary>
[PreferenceItemConverter<bool, bool>]
public sealed class ToggleSwitchReceivingBooleanConverter : PreferenceItemConverter<ToggleSwitch>
{
	/// <inheritdoc/>
	public override bool Bind(object? value, ToggleSwitch control)
	{
		if (value is bool v)
		{
			control.IsOn = v;
			return true;
		}

		return false;
	}
}
