namespace Sudoku.Bot.Communication.Models.Interaction;

/// <summary>
/// Indicates which fields will be required to set. The filter will be used for creating a role for inferring the values.
/// </summary>
public sealed class Filter
{
	/// <summary>
	/// Initializes a <see cref="Filter"/> instance.
	/// </summary>
	/// <param name="setName">Indicates the name.</param>
	/// <param name="setColor">Indicates the color.</param>
	/// <param name="setHoist">Indicates whether the current role is displayed alone.</param>
	public Filter(bool setName = false, bool setColor = false, bool setHoist = false)
		=> (Name, Color, Hoist) = (setName, setColor, setHoist);


	/// <summary>
	/// Indicates whether the creation will set the name.
	/// </summary>
	[JsonPropertyName("name"), JsonConverter(typeof(BoolInt32Converter))]
	public bool Name { get; set; }

	/// <summary>
	/// Indicates whether the creation will set the color.
	/// </summary>
	[JsonPropertyName("color"), JsonConverter(typeof(BoolInt32Converter))]
	public bool Color { get; set; }

	/// <summary>
	/// Indicates whether the creation will set the hoist value.
	/// </summary>
	[JsonPropertyName("hoist"), JsonConverter(typeof(BoolInt32Converter))]
	public bool Hoist { get; set; }
}
