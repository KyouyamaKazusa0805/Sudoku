namespace Sudoku.Bot.Oicq.Extensibility;

/// <summary>
/// Provides a converter that can convert the <see cref="AppInfo"/> instance into MyQQ-formatted JSON string value.
/// </summary>
public sealed class AppInfoConverter : IAppInfoConverter
{
	/// <inheritdoc/>
	public string Convert(AppInfo info)
	{
		using var stream = new MemoryStream();
		var options = new JsonWriterOptions { Indented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
		var json = new Utf8JsonWriter(stream, options);
		json.WriteStartObject();
		json.WriteString("name", info.Name);
		json.WriteString("version", info.Version);
		json.WriteString("skey", "SDG5D4Ys89h7DJ849d");
		json.WriteString("author", info.Author);
		json.WriteString("description", info.Description);
		json.WriteString("sdk", typeof(AppInfoConverter).Assembly.GetName().Version.ToString());
		json.WriteEndObject();
		json.Flush();
		json.Dispose();

		return Encoding.UTF8.GetString(stream.ToArray());
	}
}
