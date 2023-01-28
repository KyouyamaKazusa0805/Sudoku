namespace SudokuStudio.Markup;

/// <summary>
/// Defines a <see cref="DashArray"/> markup extension.
/// </summary>
[ContentProperty(Name = nameof(Expression))]
[MarkupExtensionReturnType(ReturnType = typeof(DashArray))]
public sealed class DashArrayExtension : MarkupExtension
{
	/// <summary>
	/// The text.
	/// </summary>
	public string Expression { get; set; } = string.Empty;


	/// <inheritdoc/>
	protected override object ProvideValue()
	{
		return Expression switch
		{
			"" => new DashArray(),
			string s => f(s),
			_ => throw new InvalidOperationException("Cannot parse the string into the target result.")
		};


		static DashArray f(string s)
		{
			using scoped var result = new ValueList<double>(byte.MaxValue);

			var split = s.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			foreach (var elementRawValue in split)
			{
				if (!double.TryParse(elementRawValue, out var element))
				{
					throw new InvalidOperationException("The target value cannot be parsed into a valid double value.");
				}

				result.Add(element);
			}

			return new DashArray(result.ToArray());
		}
	}
}
