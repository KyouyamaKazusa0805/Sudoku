namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides extension methods for <see cref="INamedTypeSymbol"/>.
/// </summary>
/// <seealso cref="INamedTypeSymbol"/>
internal static class INamedTypeSymbolExtensions
{
	/// <summary>
	/// Get the file name of the type symbol.
	/// </summary>
	/// <param name="this">The symbol.</param>
	/// <returns>
	/// The file name. Due to the limited file name and the algorithm, if:
	/// <list type="bullet">
	/// <item>
	/// The character is <c><![CDATA['<']]></c> or <c><![CDATA['>']]></c>:
	/// Change them to <c>'['</c> and <c>']'</c>.
	/// </item>
	/// <item>The character is <c>','</c>: Change it to <c>'_'</c>.</item>
	/// <item>The character is <c>' '</c>: Remove it.</item>
	/// </list>
	/// </returns>
	internal static string ToFileName(this INamedTypeSymbol @this)
	{
		string result = @this.ToDisplayString(TypeFormats.FullNameWithConstraints);
		var buffer = (stackalloc char[result.Length]);
		buffer.Fill('\0');
		int pointer = 0;
		for (int i = 0, length = result.Length; i < length; i++)
		{
			switch (result[i])
			{
				case '<': { buffer[pointer++] = '['; break; }
				case '>': { buffer[pointer++] = ']'; break; }
				case ',': { buffer[pointer++] = '_'; break; }
				case ' ' or ':': { continue; }
				default: { buffer[pointer++] = result[i]; break; }
			}
		}

		return buffer[..pointer].ToString();
	}
}
