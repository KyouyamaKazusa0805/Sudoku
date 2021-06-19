using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Sudoku.CodeGen.ResourceDictionary
{
	/// <summary>
	/// Indicates the source generator that generates the code for the resource dictionary.
	/// </summary>
	[Generator]
	public sealed class ResourceDictionaryGenerator : ISourceGenerator
	{
		/// <summary>
		/// Indicates the regular expression to match a key.
		/// </summary>
		private static readonly Regex Regex = new(
			@"""(\w+)""(?=\:\s*""[^""]+"",?)",
			RegexOptions.CultureInvariant,
			TimeSpan.FromSeconds(5)
		);


		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			if (context.AdditionalFiles is not { Length: not 0 } additionalFiles)
			{
				return;
			}

			string keys = string.Join(
				",\r\n\t\t\t",
				(
					from additionalFile in additionalFiles
					from line in File.ReadLines(additionalFile.Path)
					where !string.IsNullOrWhiteSpace(line)
					let match = Regex.Match(line)
					where match.Success
					select match.Value
				).Distinct()
			);

			context.AddSource(
				"ResourceDictionaryValues.g.cs",
				SourceText.From(
					$@"#pragma warning disable 1591

using System.Collections.Generic;

#nullable enable

namespace Sudoku.CodeGen
{{
	/// <summary>
	/// Indicates the resource dictionary to check.
	/// </summary>
	internal static class ResourceDictionaryKeys
	{{
		/// <summary>
		/// Indicates the dictionary.
		/// </summary>
		public static readonly IReadOnlyCollection<string> Keys = new[]
		{{
			{keys}
		}};
	}}
}}",
					Encoding.Unicode
				)
			);
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}
	}
}
