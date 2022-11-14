namespace Sudoku.CommandLine.ValueConverters;

/// <summary>
/// Represents a converter that can convert the <see cref="string"/> value to the <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the target enumeration.</typeparam>
public sealed class EnumTypeConverter<T> : IValueConverter where T : unmanaged, Enum
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


		static bool viaName(string value, out T result)
		{
			SkipInit(out result);
			if (!int.TryParse(value, out var targetValue))
			{
				return false;
			}

			var checkType = As<int, T>(ref targetValue);
			if (Enum.IsDefined(checkType))
			{
				result = checkType;
				return true;
			}

			return false;
		}

		static bool viaAttribute(string value, out T result)
		{
			SkipInit(out result);
			foreach (var fieldInfo in typeof(T).GetFields())
			{
				var attr = fieldInfo.GetCustomAttribute<SupportedArgumentsAttribute>();
				if (attr is not { SupportedArguments: var supportedNames, IgnoreCase: var ignoreCase })
				{
					continue;
				}

				var comparisonOption = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
				if (supportedNames.All(e => !e.Equals(value, comparisonOption)))
				{
					continue;
				}

				result = Enum.Parse<T>(fieldInfo.Name);
				return true;
			}

			return false;
		}
	}
}
