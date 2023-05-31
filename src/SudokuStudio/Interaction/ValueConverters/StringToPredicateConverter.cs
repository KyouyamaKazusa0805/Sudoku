﻿namespace SudokuStudio.Interaction.ValueConverters;

/// <summary>
/// Converts a <see cref="string"/> value into a predicate that can return a <see cref="bool"/> result.
/// </summary>
public sealed class StringToPredicateConverter : IValueConverter
{
	/// <summary>
	/// Indicates whether the value conversion is reverted.
	/// </summary>
	public bool IsInverted { get; set; }

	/// <summary>
	/// Indicates whether the converter will handle for whitespaces.
	/// </summary>
	public bool EnforceNonWhiteSpaceString { get; set; }


	/// <inheritdoc/>
	public unsafe object Convert(object? value, Type? targetType, object? parameter, string? language)
		=> value is string target
		&& (delegate*<string, bool>)(EnforceNonWhiteSpaceString ? &string.IsNullOrWhiteSpace : &string.IsNullOrEmpty) is var method
			? IsInverted
				? !method(target)
				: method(target)
			: IsInverted
				? value is not null
				: value is null;

	/// <inheritdoc/>
	[DoesNotReturn]
	public object ConvertBack(object? value, Type? targetType, object? parameter, string? language) => throw new NotImplementedException();
}
