namespace Sudoku.Text;

partial struct ResourceDocument
{
	/// <summary>
	/// Indicates the enumerator of the current instance to iterate.
	/// </summary>
	/// <remarks><i>
	/// Due to the .NET API design, we can't customize to fetch the inner value, so the data structure
	/// will bind a <see cref="JsonElement.ObjectEnumerator"/> instance to iterate. For more information,
	/// please visit the documentation of method <see cref="JsonElement.EnumerateObject"/> and the type
	/// <see cref="JsonElement.ObjectEnumerator"/>.
	/// </i></remarks>
	/// <seealso cref="JsonElement.EnumerateObject"/>
	/// <seealso cref="JsonElement.ObjectEnumerator"/>
	public ref partial struct Enumerator
	{
		/// <summary>
		/// Indicates the first element to iterate.
		/// </summary>
		private JsonElement.ObjectEnumerator _enumerator;


		/// <summary>
		/// Initializes an <see cref="Enumerator"/> instance via the first element.
		/// </summary>
		/// <param name="firstElement">The first element to iterate.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator(JsonElement firstElement) => _enumerator = firstElement.EnumerateObject();


		/// <inheritdoc cref="IEnumerator{T}.Current"/>
		public readonly string Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _enumerator.Current.Value.GetString()!;
		}


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => _enumerator.MoveNext();
	}
}
