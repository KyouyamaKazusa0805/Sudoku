namespace Sudoku.UI.Models;

/// <summary>
/// Defines a floating-point or integer option that is binding with a slider on displaying.
/// </summary>
public sealed class SliderSettingItem : SettingItem
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SliderSettingItem(string name, string preferenceValueName) : base(name, preferenceValueName)
	{
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SliderSettingItem(string name, string description, string preferenceValueName)
		: base(name, description, preferenceValueName)
	{
	}


	/// <summary>
	/// Indicates the step frequency.
	/// </summary>
	public double StepFrequency { get; set; }

	/// <summary>
	/// Indicates the tick frequency.
	/// </summary>
	public double TickFrequency { get; set; }

	/// <summary>
	/// Indicates the minimum value of the slider.
	/// </summary>
	public double MinValue { get; set; }

	/// <summary>
	/// Indicates the maximum value of the slider.
	/// </summary>
	public double MaxValue { get; set; }


	/// <inheritdoc cref="SettingItem.GetPreference{T}"/>
	public double GetPreference() => GetPreference<double>();

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	public void SetPreference(double value) => SetPreference<double>(value);

	/// <summary>
	/// Gets the string result of the preference value.
	/// </summary>
	/// <returns>The string result.</returns>
	public string PreferenceValueToString() => GetPreference<double>().ToString();
}
