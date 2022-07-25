namespace Sudoku.UI.Models.SettingItems;

/// <summary>
/// Defines an integer option that is binding with a slider on displaying.
/// </summary>
public sealed class Int32SliderSettingItem : SettingItem, IDynamicCreatableItem<Int32SliderSettingItem>
{
	/// <summary>
	/// Indicates the step frequency.
	/// </summary>
	public required int StepFrequency { get; set; }

	/// <summary>
	/// Indicates the tick frequency.
	/// </summary>
	public required int TickFrequency { get; set; }

	/// <summary>
	/// Indicates the minimum value of the slider.
	/// </summary>
	public required int MinValue { get; set; }

	/// <summary>
	/// Indicates the maximum value of the slider.
	/// </summary>
	public required int MaxValue { get; set; }


	/// <inheritdoc/>
	public static Int32SliderSettingItem CreateInstance(string propertyName)
	{
		var result = NamedValueLookup.GetAttributeArguments<Int32SliderSettingItem>(propertyName) switch
		{
			{ Data: { Length: <= 4 } data } => new Int32SliderSettingItem
			{
				Name = NamedValueLookup.GetItemNameString(propertyName),
				PreferenceValueName = propertyName,
				StepFrequency = data.GetNamedValue<int>(nameof(StepFrequency)),
				TickFrequency = data.GetNamedValue<int>(nameof(TickFrequency)),
				MinValue = data.GetNamedValue<int>(nameof(MinValue)),
				MaxValue = data.GetNamedValue<int>(nameof(MaxValue))
			}
		};

		if (NamedValueLookup.GetItemDescriptionString(propertyName) is { } description)
		{
			result.Description = description;
		}

		return result;
	}

	/// <inheritdoc cref="SettingItem.GetPreference{T}()"/>
	public double GetPreference() => GetPreference<int>();

	/// <inheritdoc cref="SettingItem.SetPreference{T}(T)"/>
	public void SetPreference(double value) => SetPreference<int>((int)value);
}
