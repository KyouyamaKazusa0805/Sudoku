namespace Sudoku.UI.Models.SettingItems;

/// <summary>
/// Defines a floating-point or integer option that is binding with a slider on displaying.
/// </summary>
public sealed class SliderSettingItem : SettingItem
{
	/// <summary>
	/// Indicates the step frequency.
	/// </summary>
	public required double StepFrequency { get; set; }

	/// <summary>
	/// Indicates the tick frequency.
	/// </summary>
	public required double TickFrequency { get; set; }

	/// <summary>
	/// Indicates the minimum value of the slider.
	/// </summary>
	public required double MinValue { get; set; }

	/// <summary>
	/// Indicates the maximum value of the slider.
	/// </summary>
	public required double MaxValue { get; set; }


	/// <inheritdoc/>
	public override SliderSettingItem DynamicCreate(string propertyName)
		=> GetAttributeArguments(propertyName) switch
		{
			PreferenceAttribute<SliderSettingItem> { Data: { Length: <= 4 } data }
				=> new()
				{
					Name = GetItemNameString(propertyName)!,
					Description = GetItemDescriptionString(propertyName) ?? string.Empty,
					PreferenceValueName = propertyName,
					StepFrequency = (double)data.First(p => p.Key == nameof(StepFrequency)).Value!,
					TickFrequency = (double)data.First(p => p.Key == nameof(TickFrequency)).Value!,
					MinValue = (double)data.First(p => p.Key == nameof(MinValue)).Value!,
					MaxValue = (double)data.First(p => p.Key == nameof(MaxValue)).Value!
				},
			_ => throw new InvalidOperationException()
		};

	/// <inheritdoc cref="SettingItem.GetPreference{T}()"/>
	public double GetPreference() => GetPreference<double>();

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	public void SetPreference(double value) => SetPreference<double>(value);
}
