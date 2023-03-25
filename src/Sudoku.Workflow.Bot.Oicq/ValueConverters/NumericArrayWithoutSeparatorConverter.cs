namespace Sudoku.Workflow.Bot.Oicq.ValueConverters;

/// <summary>
/// 一个转换器类型，可以提供将 <see cref="string"/> 类型的实例解析，并转换为一个数值的类型为元素类型的数组。输入字符串不需要任何分隔符。
/// </summary>
/// <typeparam name="T">目标结果（数组）的元素的类型。</typeparam>
public sealed class NumericArrayWithoutSeparatorConverter<T> : IValueConverter where T : unmanaged, INumber<T>
{
	/// <inheritdoc/>
	public object Convert(string value)
	{
		var split = value.ToCharArray().Select(c => c.ToString());
		var result = new T[split.Length];
		for (var i = 0; i < split.Length; i++)
		{
			var element = split[i];
			if (!T.TryParse(element, null, out var target))
			{
				throw new CommandConverterException();
			}

			result[i] = target;
		}

		return result;
	}
}
