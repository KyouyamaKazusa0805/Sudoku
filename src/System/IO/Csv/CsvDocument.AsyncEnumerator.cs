using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;

namespace System.IO.Csv
{
	partial class CsvDocument
	{
		/// <summary>
		/// Indicates the enumerator that bounds with <see cref="GetAsyncEnumerator"/>.
		/// </summary>
		/// <seealso cref="GetAsyncEnumerator"/>
		public struct AsyncEnumerator
		{
			/// <summary>
			/// Indicates the parser.
			/// </summary>
			private readonly TextFieldParser _parser;


			/// <summary>
			/// Initializes a <see cref="AsyncEnumerator"/> instance using the specified text parser.
			/// </summary>
			/// <param name="parser">The text field parser.</param>
			public AsyncEnumerator(TextFieldParser parser) : this() => _parser = parser;


			/// <inheritdoc cref="IAsyncEnumerator{T}.Current"/>
			public string[]? Current { get; private set; }


			/// <inheritdoc cref="IAsyncEnumerator{T}.MoveNextAsync"/>
			public ValueTask<bool> MoveNextAsync()
			{
				var copied = _parser;
				if (!copied.EndOfData)
				{
					Current = copied.ReadFields();
					return new(true);
				}

				return new(false);
			}
		}
	}
}
