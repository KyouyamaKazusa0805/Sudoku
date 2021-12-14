namespace Sudoku.Text;

partial struct ResourceDocument
{
	/// <summary>
	/// Indicates the enumerator of the current instance to iterate.
	/// </summary>
	public ref partial struct Enumerator
	{
		/// <summary>
		/// Indicates the enumerator instance.
		/// </summary>
		private readonly IEnumerator<KeyValuePair<string, JsonNode?>> _enumerator;


		/// <summary>
		/// Initializes an <see cref="Enumerator"/> instance via the first element.
		/// </summary>
		/// <param name="object">The <see cref="JsonObject"/> instance to iterate.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator(JsonObject @object) => _enumerator = @object.GetEnumerator();


		/// <summary>
		/// Gets the key-value pair in the resource dictionary at the current position of the enumerator.
		/// </summary>
		public readonly (string Name, string Value) Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				var (key, value) = _enumerator.Current;
				return (key, value!.GetValue<string>());
			}
		}


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => _enumerator.MoveNext();
	}
}
