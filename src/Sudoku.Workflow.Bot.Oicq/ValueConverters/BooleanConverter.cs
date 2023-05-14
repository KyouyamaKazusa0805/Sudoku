namespace Sudoku.Workflow.Bot.Oicq.ValueConverters;

/// <summary>
/// 一个转换器类型，可以提供将 <see cref="string"/> 类型的实例解析为 <see cref="bool"/> 结果。
/// </summary>
public sealed class BooleanConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(string value)
		=> value.Trim() switch
		{
			"是" or "Y" or "y" or "Yes" or "yes" => true,
			"否" or "N" or "n" or "No" or "no" => false,
			_ => throw new CommandConverterException()
		};
}
