using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sudoku.Data
{
	partial struct Link
	{
		/// <summary>
		/// Defines a JSON converter that allows the current instance being serialized.
		/// </summary>
		public sealed unsafe class JsonConverter : JsonConverter<Link>
		{
			/// <inheritdoc/>
			public override bool HandleNull => false;


			/// <inheritdoc/>
			public override Link Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				var (start, end, linkType, propName) = default((int, int, LinkType, string?));
				while (reader.Read())
				{
					switch (reader.TokenType)
					{
						case JsonTokenType.PropertyName:
						{
							propName = reader.GetString();
							break;
						}
						case JsonTokenType.Number:
						{
							switch (propName)
							{
								case nameof(StartCandidate):
								{
									start = reader.GetInt32();
									break;
								}
								case nameof(EndCandidate):
								{
									end = reader.GetInt32();
									break;
								}
								case nameof(LinkType):
								{
									linkType = (LinkType)reader.GetByte();
									break;
								}
							}

							break;
						}
					}
				}

				return new(start, end, linkType);
			}

			/// <inheritdoc/>
			public override void Write(Utf8JsonWriter writer, Link value, JsonSerializerOptions options)
			{
				writer.WriteStartObject();
				writer.WriteNumber(nameof(StartCandidate), value.StartCandidate);
				writer.WriteNumber(nameof(EndCandidate), value.EndCandidate);
				writer.WriteNumber(nameof(LinkType), (byte)value.LinkType);
				writer.WriteEndObject();
			}
		}
	}
}
