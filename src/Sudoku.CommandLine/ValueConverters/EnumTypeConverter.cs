namespace Sudoku.CommandLine.ValueConverters;

/// <summary>
/// Represents a converter that can convert the <see cref="string"/> value to the <typeparamref name="TEnum"/>.
/// </summary>
/// <typeparam name="TEnum">The type of the target enumeration.</typeparam>
public sealed class EnumTypeConverter<TEnum> : IValueConverter where TEnum : unmanaged, Enum
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public object? Convert(string value)
	{
		if (!int.TryParse(value, out int targetValue))
		{
			return null;
		}

		var checkType = Unsafe.As<int, TEnum>(ref targetValue);
		return Enum.IsDefined(checkType) ? checkType : null;
	}
}
