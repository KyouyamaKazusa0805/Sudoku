namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code on JSON serializations for bit operations.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class Utf8JsonWriterBitOperationsGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		string[] types = { "sbyte", "byte", "short", "ushort", "int", "uint", "long", "ulong" };
		string c = string.Join(
			"\r\n\r\n\t",
			from type in types
			select $@"/// <summary>
	/// Writes the bit collection of type <see cref=""{type}""/> into the JSON stream.
	/// </summary>
	/// <param name=""this"">The writer.</param>
	/// <param name=""propertyName"">The property name.</param>
	/// <param name=""bits"">The bits to write.</param>
	public static void WriteBitCollection(this Utf8JsonWriter @this, string propertyName, {type} bits)
	{{
		@this.WritePropertyName(propertyName);
		@this.WriteStartArray();
		foreach (int bitPos in bits)
		{{
			@this.WriteNumberValue(bitPos);
		}}
		@this.WriteEndArray();
	}}"
		);

		string code = $@"#pragma warning disable CS1591

using System.Numerics;

#nullable enable

namespace System.Text.Json;

/// <summary>
/// Defines a set of methods that handles the bits and the type <see cref=""Utf8JsonWriter""/>.
/// </summary>
/// <seealso cref=""Utf8JsonWriter""/>
public static class Utf8JsonWriterBitOperationsExensions
{{
	{c}
}}";

		context.AddSource("Utf8JsonWriter", "u8wbo", code);
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
	{
	}
}
