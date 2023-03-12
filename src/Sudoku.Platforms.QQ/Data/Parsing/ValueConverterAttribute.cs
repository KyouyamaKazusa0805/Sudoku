namespace Sudoku.Platforms.QQ.Data.Parsing;

/// <summary>
/// Provides with an attribute type that describes this property value should receive a value not type <see cref="string"/> as message text,
/// the conversion is specified as type argument.
/// </summary>
/// <typeparam name="T">The type of the value converter instance.</typeparam>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class ValueConverterAttribute<T> : CommandLineParsingItemAttribute where T : class, IValueConverter, new()
{
}
