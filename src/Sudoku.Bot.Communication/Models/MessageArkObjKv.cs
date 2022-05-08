namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// ark obj键值对
/// </summary>
public class MessageArkObjKv
{
	/// <summary>
	/// 构造函数
	/// </summary>
	public MessageArkObjKv() { }
	/// <summary>
	/// ark obj键值对构造函数
	/// </summary>
	/// <param name="key"></param>
	/// <param name="value"></param>
	public MessageArkObjKv(string key, string value)
	{
		Key = key;
		Value = value;
	}
	/// <summary>
	/// 键
	/// </summary>
	[JsonPropertyName("key")]
	public string Key { get; set; } = "";
	/// <summary>
	/// 值
	/// </summary>
	[JsonPropertyName("value")]
	public string? Value { get; set; }
}
