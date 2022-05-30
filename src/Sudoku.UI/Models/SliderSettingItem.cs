namespace Sudoku.UI.Models;

/// <summary>
/// Defines a floating-point or integer option that is binding with a slider on displaying.
/// </summary>
public sealed class SliderSettingItem : SettingItem
{
	/// <inheritdoc cref="SettingItem(string, string)"/>
	/// <param name="name"><inheritdoc/></param>
	/// <param name="preferenceValueName"><inheritdoc/></param>
	/// <param name="stepFrequency">The step frequency.</param>
	/// <param name="tickFrequency">The tick frequency.</param>
	/// <param name="minValue">The minimum value.</param>
	/// <param name="maxValue">The maximum value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SliderSettingItem(
		string name, string preferenceValueName, double stepFrequency, double tickFrequency,
		double minValue, double maxValue) : base(name, preferenceValueName)
		=> (StepFrequency, TickFrequency, MinValue, MaxValue) = (stepFrequency, tickFrequency, minValue, maxValue);

	/// <inheritdoc cref="SettingItem(string, string, string)"/>
	/// <param name="name"><inheritdoc/></param>
	/// <param name="description"><inheritdoc/></param>
	/// <param name="preferenceValueName"><inheritdoc/></param>
	/// <param name="stepFrequency">The step frequency.</param>
	/// <param name="tickFrequency">The tick frequency.</param>
	/// <param name="minValue">The minimum value.</param>
	/// <param name="maxValue">The maximum value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SliderSettingItem(
		string name, string description, string preferenceValueName,
		double stepFrequency, double tickFrequency, double minValue, double maxValue) :
		base(name, description, preferenceValueName)
		=> (StepFrequency, TickFrequency, MinValue, MaxValue) = (stepFrequency, tickFrequency, minValue, maxValue);


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
}
