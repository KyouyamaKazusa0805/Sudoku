namespace Sudoku.Workflow.Bot.Oicq.ValueConverters;

/// <summary>
/// 一个转换器类型，可以提供将 <see cref="string"/> 类型的实例解析，并转换为一个数值类型的实例。
/// </summary>
/// <typeparam name="T">目标数值的类型。</typeparam>
public sealed class NumericConverter<T> : IValueConverter where T : unmanaged, INumber<T>
{
	/// <inheritdoc/>
	public object Convert(string value) => T.TryParse(value, null, out var result) ? result : throw new CommandConverterException();
}
