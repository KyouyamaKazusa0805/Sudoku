namespace Sudoku.UI.Models.SettingItems;

/// <summary>
/// Defines a floating-point or integer option that is binding with a slider on displaying.
/// </summary>
public sealed class SliderSettingItem : SettingItem, IDynamicCreatableItem<SliderSettingItem>
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
	public static SliderSettingItem CreateInstance(string propertyName)
	{
		var result = NamedValueLookup.GetAttributeArguments<SliderSettingItem>(propertyName) switch
		{
			{ Data: { Length: <= 4 } data } => new SliderSettingItem
			{
				Name = NamedValueLookup.GetItemNameString(propertyName),
				PreferenceValueName = propertyName,
				StepFrequency = data.GetNamedValue<double>(nameof(StepFrequency)),
				TickFrequency = data.GetNamedValue<double>(nameof(TickFrequency)),
				MinValue = data.GetNamedValue<double>(nameof(MinValue)),
				MaxValue = data.GetNamedValue<double>(nameof(MaxValue))
			}
		};

		if (NamedValueLookup.GetItemDescriptionString(propertyName) is { } description)
		{
			result.Description = description;
		}

		return result;
	}

	/// <inheritdoc cref="SettingItem.GetPreference{T}()"/>
	public double GetPreference() => GetPreference<double>();

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	public void SetPreference(double value) => SetPreference<double>(value);
}
