using System.Text.Json;
using System.Text.Json.Serialization;
using Sudoku.Data;
using Sudoku.Models;

namespace Sudoku.Drawing.Converters
{
	/// <summary>
	/// Indicates a <see cref="View"/> JSON converter.
	/// </summary>
	/// <seealso cref="View"/>
	[JsonConverter(typeof(View))]
	public sealed class ViewJsonConverter : JsonConverter<View>
	{
		/// <summary>
		/// Indicates the inner options.
		/// </summary>
		private static readonly JsonSerializerOptions InnerOptions;


		static ViewJsonConverter()
		{
			InnerOptions = new();
			InnerOptions.Converters.Add(new DrawingInfoJsonConverter());
			InnerOptions.Converters.Add(new LinkJsonConverter());
			InnerOptions.Converters.Add(new DirectLineJsonConverter());
		}


		/// <inheritdoc/>
		public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(View);

		/// <inheritdoc/>
		public override View? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var cells = new List<DrawingInfo>();
			var candidates = new List<DrawingInfo>();
			var regions = new List<DrawingInfo>();
			var links = new List<Link>();
			var directLines = new List<(Cells, Cells)>();

			dynamic? inst = null;
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
					case JsonTokenType.PropertyName:
					{
						inst = reader.GetString() switch
						{
							nameof(View.Cells) => cells,
							nameof(View.Candidates) => candidates,
							nameof(View.Regions) => regions,
							nameof(View.Links) => links,
							nameof(View.DirectLines) => directLines
						};
						break;
					}
					case JsonTokenType.String:
					{
						string str = reader.GetString()!;
						var type = (Type)inst!.GetType().GenericTypeArguments[0];
						object value = JsonSerializer.Deserialize(str, type, InnerOptions)!;
						if (inst.GetType() == typeof(List<DrawingInfo>))
						{
							inst.Add((DrawingInfo)value);
						}
						else if (inst.GetType() == typeof(List<Link>))
						{
							inst.Add((Link)value);
						}
						else if (inst.GetType() == typeof(List<(Cells, Cells)>))
						{
							inst.Add(((Cells, Cells))value);
						}
						else
						{
							throw new InvalidCastException();
						}
						break;
					}
				}
			}

			return new()
			{
				Cells = assign(cells),
				Candidates = assign(candidates),
				Regions = assign(regions),
				Links = assign(links),
				DirectLines = assign(directLines)
			};

			static IReadOnlyList<T>? assign<T>(List<T> z) => z.Count == 0 ? null : z;
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, View value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();

			writer.WritePropertyName(nameof(View.Cells));
			writer.WriteStartArray();
			if (value.Cells is { } cells)
			{
				foreach (var info in cells)
				{
					writer.WriteStringValue(JsonSerializer.Serialize(info, InnerOptions));
				}
			}
			writer.WriteEndArray();

			writer.WritePropertyName(nameof(View.Candidates));
			writer.WriteStartArray();
			if (value.Candidates is { } candidates)
			{
				foreach (var info in candidates)
				{
					writer.WriteStringValue(JsonSerializer.Serialize(info, InnerOptions));
				}
			}
			writer.WriteEndArray();

			writer.WritePropertyName(nameof(View.Regions));
			writer.WriteStartArray();
			if (value.Regions is { } regions)
			{
				foreach (var info in regions)
				{
					writer.WriteStringValue(JsonSerializer.Serialize(info, InnerOptions));
				}
			}
			writer.WriteEndArray();

			writer.WritePropertyName(nameof(View.Links));
			writer.WriteStartArray();
			if (value.Links is { } links)
			{
				foreach (var link in links)
				{
					writer.WriteStringValue(JsonSerializer.Serialize(link, InnerOptions));
				}
			}
			writer.WriteEndArray();

			writer.WritePropertyName(nameof(View.DirectLines));
			writer.WriteStartArray();
			if (value.DirectLines is { } lines)
			{
				foreach (var line in lines)
				{
					writer.WriteStringValue(JsonSerializer.Serialize(line, InnerOptions));
				}
			}
			writer.WriteEndArray();

			writer.WriteEndObject();
		}
	}
}
