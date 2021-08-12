namespace System.IO.Csv
{
	partial class CsvDocument
	{
		/// <summary>
		/// Indicates the enumerator that bounds with <see cref="GetEnumerator"/>.
		/// </summary>
		/// <seealso cref="GetEnumerator"/>
		public ref struct Enumerator
		{
			/// <summary>
			/// Indicates the parser.
			/// </summary>
			private readonly TextFieldParser _parser;


			/// <summary>
			/// Initializes a <see cref="Enumerator"/> instance using the specified text parser.
			/// </summary>
			/// <param name="parser">The text field parser.</param>
			public Enumerator(TextFieldParser parser) : this() => _parser = parser;


			/// <inheritdoc cref="IEnumerator.Current"/>
			public string[]? Current { get; private set; }


			/// <summary>
			/// Advances the enumerator to the next element of the collection.
			/// </summary>
			/// <returns>
			/// Returns the value if:
			/// <list type="table">
			/// <item>
			/// <term><see langword="true"/></term>
			/// <description>If the enumerator was sucessfuly advanced to the next element.</description>
			/// </item>
			/// <item>
			/// <term><see langword="false"/></term>
			/// <description>If the enumerator has passed the end of the collection.</description>
			/// </item>
			/// </list>
			/// </returns>
			public bool MoveNext()
			{
				if (!_parser.EndOfData)
				{
					Current = _parser.ReadFields();
					return true;
				}

				return false;
			}
		}
	}
}
