namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 携带需要设置的字段内容
/// </summary>
public class Info
{
	/// <summary>
	/// 构造身份组信息
	/// </summary>
	/// <param name="name">名称</param>
	/// <param name="color">颜色</param>
	/// <param name="hoist">在成员列表中单独展示</param>
	public Info(string? name = null, Color? color = null, bool? hoist = null)
		=> (Name, Color, Hoist) = (name, color, hoist);

	/// <summary>
	/// 构造身份组信息
	/// </summary>
	/// <param name="name">名称</param>
	/// <param name="colorHtml">ARGB颜色值的HTML表现形式（如：#FFFFFFFF）</param>
	/// <param name="hoist">在成员列表中单独展示</param>
	public Info(string? name = null, string? colorHtml = null, bool? hoist = null)
		=> (Name, ColorHtml, Hoist) = (name, colorHtml, hoist);


	/// <summary>
	/// 名称
	/// </summary>
	[JsonPropertyName("name")]
	public string? Name { get; set; }

	/// <summary>
	/// 颜色
	/// </summary>
	[JsonPropertyName("color"), JsonConverter(typeof(ColorToUint32Converter))]
	public Color? Color { get; set; }

	/// <summary>
	/// ARGB的HTML十六进制颜色值
	/// <para>
	/// 支持这些格式(忽略大小写和前后空白字符)：<br/>
	/// #FFFFFFFF #FFFFFF #FFFF #FFF
	/// </para>
	/// <para><em>注: 因官方API有BUG，框架暂时强制Alpha通道固定为1.0，对功能无影响。 [2021-12-21]</em></para>
	/// </summary>
	[JsonIgnore]
	public string? ColorHtml
	{
		get => Color == null ? null : ColorTranslator.ToHtml(Color.Value);

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
	/// 在成员列表中单独展示
	/// </summary>
	[JsonPropertyName("hoist"), JsonConverter(typeof(BoolToInt32Converter))]
	public bool? Hoist { get; set; }
}
