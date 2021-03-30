using Sudoku.DocComments;

namespace System.Text.Markdown
{
	/// <summary>
	/// Provides a markdown document.
	/// </summary>
	public sealed partial class Document : IEquatable<Document>
	{
		/// <summary>
		/// Indicates the inner builder.
		/// </summary>
		private readonly StringBuilder _innerBuilder = new();


		/// <inheritdoc cref="DefaultConstructor"/>
		private Document()
		{
		}


		/// <inheritdoc/>
		public override bool Equals(object? obj) => Equals(obj as Document);

		/// <inheritdoc/>
		public bool Equals(Document? other) => other is not null && _innerBuilder.Equals(other._innerBuilder);

		/// <inheritdoc/>
		public override int GetHashCode() => _innerBuilder.ToString().GetHashCode();

		/// <inheritdoc/>
		public override string ToString() => _innerBuilder.ToString();


		/// <summary>
		/// Creates an empty document.
		/// </summary>
		/// <returns>The empty document instance.</returns>
		public static Document Create() => new();


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(Document left, Document right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(Document left, Document right) => !(left == right);
	}
}
