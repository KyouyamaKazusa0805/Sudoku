namespace Sudoku.Resources;

/// <summary>
/// Defines a type that represents the resource interpolated values used by <see cref="Step.Interpolations"/>.
/// </summary>
/// <param name="LanguageName">
/// The language name of the resource. This can be used for the comparison of the current culture via type <see cref="CultureInfo"/>,
/// for example, <c>"zh-CN"</c> and <c>"en-US"</c>.
/// </param>
/// <param name="ResourcePlaceholderValues">The values of the interpolation.</param>
/// <seealso cref="Step.Interpolations"/>
/// <seealso cref="CultureInfo"/>
public readonly record struct Interpolation(string LanguageName, string[] ResourcePlaceholderValues);
