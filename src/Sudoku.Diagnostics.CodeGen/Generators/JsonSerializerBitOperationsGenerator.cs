namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code on JSON serializations
/// (serialization or deserialization) for bit operations.
/// </summary>
#if false
[Generator(LanguageNames.CSharp)]
#endif
public sealed class JsonSerializerBitOperationsGenerator : IIncrementalGenerator
{
	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(context.CompilationProvider, GenerateSource);

	private void GenerateSource(SourceProductionContext spc, Compilation compilation)
	{
		if (compilation.Assembly.Name != "SystemExtensions")
		{
			return;
		}

		GenerateReaderRelatedCodes(spc);
		GenerateWriterRelatedCodes(spc);
	}

	private void GenerateReaderRelatedCodes(SourceProductionContext spc)
	{
		var quadruples = new (string Type, string Name, string RealName, string? Conversion)[]
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
			from quadruple in quadruples
			select $$"""
				/// <summary>
				/// Reads the array of <see cref="{{quadruple.Type}}"/>s.
				/// </summary>
				/// <param name="this">The reader.</param>
				/// <returns>The array of <see cref="{{quadruple.Type}}"/>s.</returns>
				/// <exception cref="JsonException">
				/// Throws when the next token is invalid while parsing the array.
				/// </exception>
				public static {{quadruple.Type}}[] Get{{quadruple.RealName}}Array(this ref Utf8JsonReader @this)
				{
					if (!@this.Read() || @this.TokenType != JsonTokenType.StartArray)
					{
						throw new JsonException("The next token is invalid while parsing the array.");
					}
					
					var result = new List<{{quadruple.Type}}>();
					while (@this.TokenType != JsonTokenType.EndArray)
					{
						result.Add({{quadruple.Conversion}}@this.Get{{quadruple.Name}}());
						
						if (!@this.Read())
						{
							throw new JsonException("The next token is invalid while parsing the array.");
						}
					}
					
					return result.ToArray();
				}
			"""
		);

		spc.AddSource(
			$"{nameof(Shortcuts.Utf8JsonReader)}.g.{Shortcuts.Utf8JsonReader}.cs",
			$$"""
			#pragma warning disable CS1591
			#nullable enable
			
			using System.Numerics;
			
			namespace System.Text.Json;
			
			/// <summary>
			/// Defines a set of methods that handles the bits and the type <see cref="Utf8JsonReader"/>.
			/// </summary>
			/// <seealso cref="Utf8JsonReader"/>
			public static class Utf8JsonReaderBitOperationsExensions
			{
				{{c}}
			}
			"""
		);
	}

	private void GenerateWriterRelatedCodes(SourceProductionContext spc)
	{
		string[] types = { "sbyte", "byte", "short", "ushort", "int", "uint", "long", "ulong" };
		string c = string.Join(
			"\r\n\r\n\t",
			from type in types
			select $$"""
				/// <summary>
				/// Writes the bit collection of type <see cref="{{type}}"/> into the JSON stream.
				/// </summary>
				/// <param name="this">The writer.</param>
				/// <param name="propertyName">The property name.</param>
				/// <param name="bits">The bits to write.</param>
				public static void WriteBitCollection(this Utf8JsonWriter @this, string propertyName, {{type}} bits)
				{
					@this.WritePropertyName(propertyName);
					@this.WriteStartArray();
					foreach (int bitPos in bits)
					{
						@this.WriteNumberValue(bitPos);
					}
						
					@this.WriteEndArray();
				}
			"""
		);

		spc.AddSource(
			$"{nameof(Shortcuts.Utf8JsonWriter)}.g.{Shortcuts.Utf8JsonWriter}.cs",
			$$"""
			#pragma warning disable CS1591
			#nullable enable
			
			using System.Numerics;
			
			namespace System.Text.Json;
			
			/// <summary>
			/// Defines a set of methods that handles the bits and the type <see cref="Utf8JsonWriter"/>.
			/// </summary>
			/// <seealso cref="Utf8JsonWriter"/>
			public static class Utf8JsonWriterBitOperationsExensions
			{
				{{c}}
			}
			"""
		);
	}
}
