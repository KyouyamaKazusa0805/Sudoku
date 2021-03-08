using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace System.Text
{
	partial struct ValueStringBuilder
	{
		/// <inheritdoc/>
		[DoesNotReturn, EditorBrowsable(EditorBrowsableState.Never)]
		public override readonly bool Equals(object? obj) => throw new NotImplementedException();

		/// <inheritdoc/>
		[DoesNotReturn, EditorBrowsable(EditorBrowsableState.Never)]
		public override readonly int GetHashCode() => throw new NotImplementedException();
	}
}
