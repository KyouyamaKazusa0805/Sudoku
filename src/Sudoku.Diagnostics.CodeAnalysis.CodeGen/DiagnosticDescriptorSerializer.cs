namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// The internal serializer that checks and deserializes the file, parsing it into a valid <see cref="DiagnosticDescriptor"/> instance.
/// </summary>
/// <seealso cref="DiagnosticDescriptor"/>
internal static class DiagnosticDescriptorSerializer
{
	/// <summary>
	/// Try to deserialize from file, and parse it, then convert it into valid <see cref="DiagnosticDescriptor"/> instances.
	/// </summary>
	/// <param name="fileName">The file name.</param>
	/// <returns>The <see cref="DiagnosticDescriptor"/> instances.</returns>
	/// <exception cref="NotSupportedException">
	/// Throws when parser encountered quote token <c>"</c> in a line while parsing.
	/// </exception>
	/// <exception cref="FormatException">
	/// Throws when a line does not contain enough data, or the current line contains some invalid data.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	/// Throws when title or severity part is <see langword="null"/> or empty spaces.
	/// </exception>
	public static DiagnosticDescriptor[] GetDiagnosticDescriptorsFromFile(string fileName)
	{
		var lines = File.ReadAllText(fileName).Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

		var result = new List<DiagnosticDescriptor>();
		foreach (var line in lines.Skip(1))
		{
			if (line.IndexOf('"') != -1)
			{
				throw new NotSupportedException(
					"The current method cannot analyze the complex case that a line contains a '\"' token. Please use other tokens instead.");
			}

			if (line.Split(',') is not
				[
					['S', 'C', 'A', >= '0' and <= '9', >= '0' and <= '9', >= '0' and <= '9', >= '0' and <= '9'] id,
					var title,
					var format,
					var category,
					var severity,
					var description
				])
			{
				throw new FormatException("The format of the current line is invalid.");
			}

			result.Add(
				new(
					id.Trim(),
					string.IsNullOrWhiteSpace(title) ? throw new InvalidOperationException("The value of 'title' is invalid.") : title.Trim(),
					(string.IsNullOrWhiteSpace(format) ? title : format).Trim(),
					category.Trim(),
					Enum.TryParse<DiagnosticSeverity>(severity, out var r)
						? r
						: throw new InvalidOperationException("The value of 'severity' is invalid."),
					true,
					string.IsNullOrWhiteSpace(description)
						? $"{title.Trim()}."
						: description.Trim() switch
						{
							[.., '.' or '?'] formattedDescription => formattedDescription,
							[.., >= 'A' and <= 'Z' or >= 'a' and <= 'z'] formattedDescription => formattedDescription,
							[.. var slice, var last] when char.IsPunctuation(last) => $"{slice}."
						},
					$"https://sunnieshine.github.io/Sudoku/code-analysis/{id.ToLower()}"
				)
			);
		}

		return result.ToArray();
	}
}
