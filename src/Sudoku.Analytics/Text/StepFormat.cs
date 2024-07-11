namespace Sudoku.Text;

/// <summary>
/// Represents a resource format. This type is used by <see cref="Step"/> instances to describe the technique format
/// stored in resource dictionary.
/// </summary>
/// <param name="name">Indicates the technique identifier name.</param>
/// <seealso cref="Step"/>
public readonly struct StepFormat(string name) : IFormattable, IFormatProvider
{
	/// <summary>
	/// Full name of the format.
	/// </summary>
	private string TechniqueResourceName => $"TechniqueFormat_{name}";


	/// <summary>
	/// Indicates the format key. The value can be <see langword="null"/> if the step does not contain an equivalent resource key.
	/// </summary>
	/// <param name="formatProvider">The culture information.</param>
	public string? GetResourceFormat(IFormatProvider? formatProvider)
		=> name is not null
		&& SR.TryGet(TechniqueResourceName, out var resource, formatProvider as CultureInfo ?? CultureInfo.CurrentUICulture)
			? resource
			: null;

	/// <inheritdoc cref="object.ToString"/>
	public override string ToString() => ToString(null);

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(IFormatProvider? formatProvider) => GetResourceFormat(formatProvider) ?? "<Unspecified>";

	/// <summary>
	/// Get the format string for the current instance.
	/// </summary>
	/// <param name="culture">The culture information.</param>
	/// <param name="formatArguments">The format arguments.</param>
	/// <returns>The final result.</returns>
	/// <exception cref="ResourceNotFoundException">Throws when the specified culture doesn't contain the specified resource.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string Format(CultureInfo? culture, params string[] formatArguments)
		=> GetResourceFormat(culture) is { } p
			? string.Format(culture, p, formatArguments)
			: throw new ResourceNotFoundException(typeof(StepFormat).Assembly, TechniqueResourceName, culture);

	/// <inheritdoc/>
	object? IFormatProvider.GetFormat(Type? formatType) => formatType == typeof(StepFormat) ? this : null;

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);
}
