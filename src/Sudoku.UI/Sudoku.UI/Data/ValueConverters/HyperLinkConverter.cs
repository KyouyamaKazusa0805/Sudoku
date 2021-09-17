namespace Sudoku.UI.Data.ValueConverters;

/// <summary>
/// Defines a converter that allows the UI uses the key to get the inner value
/// of types <see cref="Uri"/> and <see cref="string"/>.
/// </summary>
public static class HyperLinkConverter
{
	/// <summary>
	/// Indicates the binding flags to fetch in the below methods.
	/// </summary>
	private const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;


	/// <summary>
	/// Try to convert the specified value to a <see cref="Uri"/>.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>
	/// The <see cref="Uri"/> instance. If failed to convert, <see langword="null"/> will be returned.
	/// </returns>
	[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
	[return: NotNullIfNotNull("value")]
	public static Uri? FetchUri(string? value) => FetchUri((object?)value);

	/// <summary>
	/// Try to convert the specified value to a <see cref="Uri"/>.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>
	/// The <see cref="Uri"/> instance. If failed to convert, <see langword="null"/> will be returned.
	/// </returns>
	[return: NotNullIfNotNull("value")]
	public static Uri? FetchUri(object? value) =>
		value is string str
			? typeof(HyperLinks).GetField(str, PublicStatic)?.GetValue(null) as Uri
			: null;

	/// <summary>
	/// Try to convert the specified value to a <see cref="string"/>.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>
	/// The <see cref="string"/> instance. If failed to convert, <see langword="null"/> will be returned.
	/// </returns>
	[return: NotNullIfNotNull("value")]
	public static string? FetchString(object? value) =>
		value is string str
			? typeof(HyperLinks).GetField($"{str}Link", PublicStatic)?.GetValue(null) as string
			: null;
}
