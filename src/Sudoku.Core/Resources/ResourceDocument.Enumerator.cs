namespace Sudoku.Resources;

partial class ResourceDocument
{
	/// <summary>
	/// Indicates the enumerator of the current instance to iterate.
	/// </summary>
	public ref partial struct Enumerator
	{
		/// <summary>
		/// Indicates the enumerator instance.
		/// </summary>
		private JsonElement.ObjectEnumerator _enumerator;


		/// <summary>
		/// Initializes an <see cref="Enumerator"/> instance via the first element.
		/// </summary>
		/// <param name="root">The root <see cref="JsonElement"/> instance to iterate.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator(JsonElement root) => _enumerator = root.EnumerateObject();


		/// <summary>
		/// Gets the key-value pair in the resource dictionary at the current position of the enumerator.
		/// </summary>
		public readonly (string Name, string Value) Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				_ = _enumerator.Current is { Name: var name, Value: var value };
				return (name, value.GetString()!);
			}
		}


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => _enumerator.MoveNext();
	}
}
