namespace Sudoku.Workflow.Bot.Oicq.ValueConverters;

/// <summary>
/// 一个转换器类型，可以提供将 <see cref="string"/> 类型的实例解析，并转换为一个数值的类型为元素类型的数组。
/// </summary>
/// <typeparam name="T">目标结果（数组）的元素的类型。</typeparam>
public sealed class NumericArrayConverter<T> : IValueConverter where T : unmanaged, INumber<T>
{
	/// <summary>
	/// 表示用于分割字符串的记号。
	/// </summary>
	private static readonly char[] SplitCharacters = { ',', '，', '、' };


	/// <inheritdoc/>
	public object Convert(string value)
	{
		var split = value.Split(SplitCharacters, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
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
