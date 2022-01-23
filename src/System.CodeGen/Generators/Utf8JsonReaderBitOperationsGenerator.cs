namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code on JSON serializations for bit operations.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class Utf8JsonReaderBitOperationsGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var quadriples = new (string Type, string Name, string RealName, string? Conversion)[]
		{
			("sbyte", "Int32", "SByte", "(sbyte)"),
			("byte", "Int32", "Byte", "(byte)"),
			("short", "Int16", "Int16", null),
			("ushort", "UInt16", "UInt16", null),
			("int", "Int32", "Int32", null),
			("uint", "UInt32", "UInt32", null),
			("long", "Int64", "Int64", null),
			("ulong", "UInt64", "UInt64", null)
		};
		string c = string.Join(
			"\r\n\r\n\t",
			from quadruple in quadriples
			select $@"/// <summary>
	/// Reads the array of <see cref=""{quadruple.Type}""/>s.
	/// </summary>
	/// <param name=""this"">The reader.</param>
	/// <returns>The array of <see cref=""{quadruple.Type}""/>s.</returns>
	/// <exception cref=""JsonException"">
	/// Throws when the next token is invalid while parsing the array.
	/// </exception>
	public static {quadruple.Type}[] Get{quadruple.RealName}Array(this ref Utf8JsonReader @this)
	{{
		if (!@this.Read() || @this.TokenType != JsonTokenType.StartArray)
		{{
			throw new JsonException(""The next token is invalid while parsing the array."");
		}}

		var result = new List<{quadruple.Type}>();
		while (@this.TokenType != JsonTokenType.EndArray)
		{{
			result.Add({quadruple.Conversion}@this.Get{quadruple.Name}());

			if (!@this.Read())
			{{
				throw new JsonException(""The next token is invalid while parsing the array."");
			}}
		}}

		return result.ToArray();
	}}"
		);

		string code = $@"#pragma warning disable CS1591

using System.Numerics;

#nullable enable

namespace System.Text.Json;

/// <summary>
/// Defines a set of methods that handles the bits and the type <see cref=""Utf8JsonReader""/>.
/// </summary>
/// <seealso cref=""Utf8JsonReader""/>
public static class Utf8JsonReaderBitOperationsExensions
{{
	{c}
}}";

		context.AddSource("Utf8JsonReader", "u8rbo", code);
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
	{
	}
}
