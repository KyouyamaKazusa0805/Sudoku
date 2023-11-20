using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.SourceGeneration;
using static SudokuStudio.Strings.StringsAccessor;

namespace SudokuStudio.Strings;

/// <summary>
/// Represents a mechanism that allows you using interpolated string syntax to fetch resource text.
/// </summary>
/// <param name="literalLength">Indicates the whole length of the interpolated string.</param>
/// <param name="formattedCount">Indicates the number of the interpolated holes.</param>
/// <remarks>
/// Usage:
/// <code><![CDATA[
/// scoped ResourceFetcher expr = $"{value:ResourceKeyName}";
/// string s = expr.ToString();
/// ]]></code>
/// Here <c>value</c> will be inserted into the resource, whose related key value is specified as <c>ResourceKeyName</c> after colon token.
/// Its equivalent value is
/// <code><![CDATA[
/// string s = string.Format(GetString("ResourceKeyName"), value);
/// ]]></code>
/// This type is implemented via an interpolated string handler pattern, same as <see cref="DefaultInterpolatedStringHandler"/>,
/// marked with <see cref="InterpolatedStringHandlerAttribute"/>.
/// </remarks>
/// <seealso cref="DefaultInterpolatedStringHandler"/>
/// <seealso cref="InterpolatedStringHandlerAttribute"/>
[InterpolatedStringHandler]
[StructLayout(LayoutKind.Auto)]
internal ref partial struct ResourceFetcher(
	[Data(DataMemberKinds.Field)] int literalLength,
	[Data(DataMemberKinds.Field)] int formattedCount
)
{
	/// <summary>
	/// The internal format.
	/// </summary>
	private string? _format;

	/// <summary>
	/// The internal content.
	/// </summary>
	private object? _content;


	/// <inheritdoc cref="object.ToString"/>
	/// <exception cref="InvalidOperationException">Throws when the value is not initialized.</exception>
	public override readonly string ToString()
		=> _format switch
		{
			not null => _content switch
			{
				var (a, b, c) => string.Format(GetString(_format), a, b, c),
				var (a, b) => string.Format(GetString(_format), a, b),
				ITuple tuple => string.Format(GetString(_format), tuple.ToArray()),
				not null => string.Format(GetString(_format), _content),
				_ => GetString(_format)
			},
			_ => throw new InvalidOperationException("The format cannot be null.")
		};

	/// <inheritdoc cref="DefaultInterpolatedStringHandler.AppendFormatted{T}(T, string?)"/>
	public void AppendFormatted(object? content, string format) => (_format, _content) = (format, content);
}
