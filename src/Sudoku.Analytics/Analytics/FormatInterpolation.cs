using System.Globalization;

namespace Sudoku.Analytics;

/// <summary>
/// Defines a type that represents the resource interpolated values used by <see cref="Step.FormatInterpolationParts"/>.
/// </summary>
/// <param name="LanguageNameOrIdentifier">
/// The language name of the resource. This can be used for the comparison of the current culture via type <see cref="CultureInfo"/>,
/// for example, <c>"zh"</c> and <c>"en-US"</c>
/// </param>
/// <param name="ResourcePlaceholderValues">The values of the interpolation.</param>
public readonly record struct FormatInterpolation(string LanguageNameOrIdentifier, string[]? ResourcePlaceholderValues);
