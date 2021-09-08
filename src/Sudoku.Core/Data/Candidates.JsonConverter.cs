namespace Sudoku.Data;

partial struct Candidates
{
	/// <summary>
	/// Defines a JSON converter that allows the current instance being serialized.
	/// </summary>
	[JsonConverter(typeof(Candidates))]
	public sealed class JsonConverter : JsonConverter<Candidates>
	{
		/// <inheritdoc/>
		public override bool HandleNull => false;


		/// <inheritdoc/>
		/// <exception cref="InvalidOperationException">
		/// Throws when the specified part contains the invalid data (such as the wrong type) while reading.
		/// </exception>
		public override unsafe Candidates Read(
			ref Utf8JsonReader reader,
			Type typeToConvert,
			JsonSerializerOptions options
		)
		{
			byte pos;
			long _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11;
			try
			{
				while (reader.Read())
				{
					switch (reader.TokenType)
					{
						case JsonTokenType.PropertyName:
						{
							pos = reader.GetString() switch
							{
								"Part1" => 0,
								"Part2" => 1,
								"Part3" => 2,
								"Part4" => 3,
								"Part5" => 4,
								"Part6" => 5,
								"Part7" => 6,
								"Part8" => 7,
								"Part9" => 8,
								"Part10" => 9,
								"Part11" => 10,
								"Part12" => 11
							};

							break;
						}
						case JsonTokenType.Number:
						{
							*(*&pos switch
							{
								0 => &_0,
								1 => &_1,
								2 => &_2,
								3 => &_3,
								4 => &_4,
								5 => &_5,
								6 => &_6,
								7 => &_7,
								8 => &_8,
								9 => &_9,
								10 => &_10,
								11 => &_11
							}) = reader.GetInt64();
							break;
						}
					}
				}
			}
			catch (InvalidOperationException ex)
			{
				throw new InvalidOperationException($"Data at the specified part is invalid: 'Part{*&pos + 1}'.", ex);
			}

			return new(*&_0, *&_1, *&_2, *&_3, *&_4, *&_5, *&_6, *&_7, *&_8, *&_9, *&_10, *&_11);
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, Candidates value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteNumber("Part1", value._0);
			writer.WriteNumber("Part2", value._1);
			writer.WriteNumber("Part3", value._2);
			writer.WriteNumber("Part4", value._3);
			writer.WriteNumber("Part5", value._4);
			writer.WriteNumber("Part6", value._5);
			writer.WriteNumber("Part7", value._6);
			writer.WriteNumber("Part8", value._7);
			writer.WriteNumber("Part9", value._8);
			writer.WriteNumber("Part10", value._9);
			writer.WriteNumber("Part11", value._10);
			writer.WriteNumber("Part12", value._11);
			writer.WriteEndObject();
		}
	}
}
