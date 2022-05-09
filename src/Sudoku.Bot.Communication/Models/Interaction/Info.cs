namespace Sudoku.Bot.Communication.Models.Interaction;

/// <summary>
/// Defines a type that stores the data that can be taken by the message.
/// </summary>
public sealed class Info
{
	/// <summary>
	/// Initializes a <see cref="Info"/> instance via the specified name, color and a <see cref="bool"/>
	/// value indicating whether the user is hoisted.
	/// </summary>
	/// <param name="name">The name.</param>
	/// <param name="color">The color.</param>
	/// <param name="isHoisted">Indicates whether the user is hoisted.</param>
	public Info(string? name = null, Color? color = null, bool? isHoisted = null)
		=> (Name, Color, Hoist) = (name, color, isHoisted);

	/// <summary>
	/// Initializes a <see cref="Info"/> instance via the specified name, color and a <see cref="bool"/>
	/// value indicating whether the user is hoisted.
	/// </summary>
	/// <param name="name">The name.</param>
	/// <param name="colorHtml">The color, as hex string representation.</param>
	/// <param name="isHoisted">Indicates whether the user is hoisted.</param>
	public Info(string? name = null, string? colorHtml = null, bool? isHoisted = null)
		=> (Name, ColorHtml, Hoist) = (name, colorHtml, isHoisted);


	/// <summary>
	/// Indicates the name of the user.
	/// </summary>
	[JsonPropertyName("name")]
	public string? Name { get; set; }

	/// <summary>
	/// Indicates the color of the user being displayed.
	/// </summary>
	[JsonPropertyName("color"), JsonConverter(typeof(ColorUInt32Converter))]
	public Color? Color { get; set; }

	/// <summary>
	/// Indicates the color of the user being displayed, as hex string representation.
	/// All possible formats are:
	/// <list type="bullet">
	/// <item>ARGB format (e.g. <c>#FFFFFFFF</c>)</item>
	/// <item>RGB format (e.g. <c>#FFFFFF</c>)</item>
	/// <item>Duplicated ARGB format (e.g. <c>#FFFF</c>)</item>
	/// <item>Duplicated RGB format (e.g. <c>#FFF</c>)</item>
	/// </list>
	/// </summary>
	/// <remarks>
	/// <b>
	/// Please note that due to the bug waiting for being fixed by the offical APIs,
	/// the framework doesn't support alpha value. The alpha value will be set to 1.0 as the constant value.
	/// </b>
	/// (note date: 2021/12/21)
	/// </remarks>
	[JsonIgnore]
	public string? ColorHtml
	{
		get => Color is null ? null : ColorTranslator.ToHtml(Color.Value);

		set
		{
			value = value switch
			{
				['0', 'x', .. var otherChars] => otherChars,
				['#', .. var otherChars] => otherChars,
				[_, var a, var b, var c] => $"#{new string(a, 2)}{new string(b, 2)}{new string(c, 2)}",
				[_, var a, var b, var c, var d] => $"#{new string(a, 2)}{new string(b, 2)}{new string(c, 2)}{new string(d, 2)}",
				_ => value
			};

			Color = string.IsNullOrWhiteSpace(value) ? null : ColorTranslator.FromHtml(value);
		}
	}

	/// <summary>
	/// Indicates whether the user is hoisted.
	/// </summary>
	[JsonPropertyName("hoist"), JsonConverter(typeof(BoolInt32Converter))]
	public bool? Hoist { get; set; }
}
