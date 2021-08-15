using Sudoku.Drawing.Old;

namespace Sudoku.Drawing.Converters.Old;

/// <summary>
/// Indicates a <see cref="MutableView"/> JSON converter.
/// </summary>
/// <seealso cref="MutableView"/>
[JsonConverter(typeof(MutableView))]
public sealed class MutableViewJsonConverter : JsonConverter<MutableView>
{
	/// <summary>
	/// Indicates the inner options.
	/// </summary>
	private static readonly JsonSerializerOptions InnerOptions;


	static MutableViewJsonConverter()
	{
		InnerOptions = new();
		InnerOptions.Converters.Add(new DrawingInfoJsonConverter());
		InnerOptions.Converters.Add(new LinkJsonConverter());
		InnerOptions.Converters.Add(new DirectLineJsonConverter());
	}


	/// <inheritdoc/>
	public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(MutableView);

	/// <inheritdoc/>
	public override MutableView? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
						nameof(MutableView.Cells) => cells,
						nameof(MutableView.Candidates) => candidates,
						nameof(MutableView.Regions) => regions,
						nameof(MutableView.Links) => links,
						nameof(MutableView.DirectLines) => directLines
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

		static ICollection<T>? assign<T>(List<T> z) => z.Count == 0 ? null : z;
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, MutableView value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();

		writer.WritePropertyName(nameof(MutableView.Cells));
		writer.WriteStartArray();
		if (value.Cells is { } cells)
		{
			foreach (var info in cells)
			{
				writer.WriteStringValue(JsonSerializer.Serialize(info, InnerOptions));
			}
		}
		writer.WriteEndArray();

		writer.WritePropertyName(nameof(MutableView.Candidates));
		writer.WriteStartArray();
		if (value.Candidates is { } candidates)
		{
			foreach (var info in candidates)
			{
				writer.WriteStringValue(JsonSerializer.Serialize(info, InnerOptions));
			}
		}
		writer.WriteEndArray();

		writer.WritePropertyName(nameof(MutableView.Regions));
		writer.WriteStartArray();
		if (value.Regions is { } regions)
		{
			foreach (var info in regions)
			{
				writer.WriteStringValue(JsonSerializer.Serialize(info, InnerOptions));
			}
		}
		writer.WriteEndArray();

		writer.WritePropertyName(nameof(MutableView.Links));
		writer.WriteStartArray();
		if (value.Links is { } links)
		{
			foreach (var link in links)
			{
				writer.WriteStringValue(JsonSerializer.Serialize(link, InnerOptions));
			}
		}
		writer.WriteEndArray();

		writer.WritePropertyName(nameof(MutableView.DirectLines));
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
