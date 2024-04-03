namespace SudokuStudio.Markup;

/// <summary>
/// Defines a markup extension that can parse a <see cref="string"/> value, converting it into a valid <see cref="int"/>
/// or throwing exceptions when the code is invalid.
/// </summary>
[ContentProperty(Name = nameof(Value))]
[MarkupExtensionReturnType(ReturnType = typeof(int))]
public sealed class Int32Extension : NumberExtension<int>;
