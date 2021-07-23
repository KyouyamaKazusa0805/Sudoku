using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGenerating.Extensions;

namespace Sudoku.CodeGenerating
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
					select Regex.Match(line) into match
					where match.Success
					select match.Value
				).Distinct()
			);

			string code = $@"using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable

namespace Sudoku.CodeGenerating
{{
	/// <summary>
	/// Indicates the resource dictionary to check.
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{Constants.Version}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	internal static class ResourceDictionaryKeys
	{{
		/// <summary>
		/// Indicates the dictionary.
		/// </summary>
		[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{Constants.Version}"")]
		[global::System.Runtime.CompilerServices.CompilerGenerated]
		public static readonly IReadOnlyCollection<string> Keys = new[]
		{{
			{keys}
		}};
	}}
}}";

			context.AddSource("ResourceDictionaryKeys", null, code, Encoding.Unicode);
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}
	}
}
