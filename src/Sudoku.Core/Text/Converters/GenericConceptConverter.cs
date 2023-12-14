namespace Sudoku.Text.Converters;

/// <summary>
/// Represents a converter that only converts from variant concept objects into corresponding equivalent <see cref="string"/> values.
/// </summary>
/// <param name="DefaultSeparator">
/// <para>Indicates the default separator. The value will be inserted into two non-digit-kind instances.</para>
/// <para>The value is <c>", "</c> by default.</para>
/// </param>
/// <param name="DigitsSeparator">
/// <para>Indicates the digits separator.</para>
/// <para>The value is <see langword="null"/> by default, meaning no separators will be inserted between 2 digits.</para>
/// </param>
public abstract record GenericConceptConverter(string DefaultSeparator = ", ", string? DigitsSeparator = null);
