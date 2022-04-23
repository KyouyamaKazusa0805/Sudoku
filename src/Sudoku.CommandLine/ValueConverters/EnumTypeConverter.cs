namespace Sudoku.CommandLine.ValueConverters;

/// <summary>
/// Represents a converter that can convert the <see cref="string"/> value to the <typeparamref name="TEnum"/>.
/// </summary>
/// <typeparam name="TEnum">The type of the target enumeration.</typeparam>
public sealed class EnumTypeConverter<TEnum> : IValueConverter where TEnum : unmanaged, Enum
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public object Convert(string value)
	{
		return viaName(value, out var namedResult)
			? namedResult
			: viaAttribute(value, out var attributeResult)
				? attributeResult
				: throw new CommandConverterException("The text cannot be parsed as the enumeration field.");


		static bool viaName(string value, out TEnum result)
		{
			Unsafe.SkipInit(out result);
			if (!int.TryParse(value, out int targetValue))
			{
				return false;
			}

			var checkType = Unsafe.As<int, TEnum>(ref targetValue);
			if (Enum.IsDefined(checkType))
			{
				result = checkType;
				return true;
			}

			return false;
		}

		static bool viaAttribute(string value, out TEnum result)
		{
			Unsafe.SkipInit(out result);
			foreach (var fieldInfo in typeof(TEnum).GetFields())
			{
				if (
					fieldInfo.GetCustomAttribute<SupportedArgumentsAttribute>() is not
					{
						SupportedArguments: var supportedNames,
						IgnoreCase: var ignoreCase
					}
				)
				{
					continue;
				}

				var comparisonOption = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
				if (supportedNames.All(e => !e.Equals(value, comparisonOption)))
				{
					continue;
				}

				result = Enum.Parse<TEnum>(fieldInfo.Name);
				return true;
			}

			return false;
		}
	}
}
