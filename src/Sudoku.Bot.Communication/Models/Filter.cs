namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 标识需要设置哪些字段
/// </summary>
public class Filter
{
	/// <summary>
	/// 配置筛选器
	/// </summary>
	/// <param name="setName">设置名称</param>
	/// <param name="setColor">设置颜色</param>
	/// <param name="setHoist">设置在成员列表中单独展示</param>
	public Filter(bool setName = false, bool setColor = false, bool setHoist = false)
	{
		Name = setName;
		Color = setColor;
		Hoist = setHoist;
	}
	/// <summary>
	/// 是否设置名称
	/// </summary>
	[JsonPropertyName("name"), JsonConverter(typeof(BoolToInt32Converter))]
	public bool Name { get; set; }
	/// <summary>
	/// 是否设置颜色
	/// </summary>
	[JsonPropertyName("color"), JsonConverter(typeof(BoolToInt32Converter))]
	public bool Color { get; set; }
	/// <summary>
	/// 是否设置在成员列表中单独展示
	/// </summary>
	[JsonPropertyName("hoist"), JsonConverter(typeof(BoolToInt32Converter))]
	public bool Hoist { get; set; }
}
