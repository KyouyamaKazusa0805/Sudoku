namespace Sudoku.Workflow.Bot.Oicq.ValueConverters;

/// <summary>
/// 表示一个转换器，将文本字符串解析为 <see cref="Grid"/> 结果。
/// </summary>
public sealed class GridConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(string value) => Grid.TryParse(value, out var result) ? result : throw new CommandConverterException();
}
