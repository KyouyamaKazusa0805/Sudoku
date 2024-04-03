namespace SudokuStudio.Markup;

/// <summary>
/// Defines a markup extension that can parse a <see cref="string"/> value, converting it into a valid <see cref="decimal"/>
/// or throwing exceptions when the code is invalid.
/// </summary>
[ContentProperty(Name = nameof(Value))]
[MarkupExtensionReturnType(ReturnType = typeof(decimal))]
public sealed class DecimalExtension : NumberExtension<decimal>;
