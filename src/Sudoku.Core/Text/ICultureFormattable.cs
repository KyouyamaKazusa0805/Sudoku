namespace Sudoku.Text;

/// <summary>
/// Represents an object that describes for an object that is defined in sudoku puzzle, with globalization (I18N) output supported.
/// </summary>
/// <seealso cref="CultureInfo"/>
public interface ICultureFormattable : IFormattable
{
	/// <summary>
	/// Converts the current object into an <see cref="string"/> result, using the specified <see cref="CultureInfo"/>
	/// to describe the data.
	/// </summary>
	/// <param name="culture">The current culture to be used.</param>
	/// <returns>The final <see cref="string"/> representation of the object.</returns>
	public abstract string ToString(CultureInfo? culture = null);

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider)
	{
		if (formatProvider is null)
		{
			goto ReturnDefaultToString;
		}

		if (formatProvider.GetFormat(typeof(ICustomFormatter)) is ICustomFormatter cf)
		{
			return cf.Format(format, this, formatProvider);
		}

		if (formatProvider.GetFormat(typeof(CultureInfo)) is CultureInfo ci)
		{
			return ToString(ci);
		}

	ReturnDefaultToString:
		return ToString();
	}
}
