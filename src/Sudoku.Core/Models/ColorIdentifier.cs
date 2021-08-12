namespace Sudoku.Models
{
	/// <summary>
	/// Indicates the color identifier that used to identify the color.
	/// </summary>
	/// <param name="UseId">Indicates whether the current instance suggests an ID using.</param>
	/// <param name="Id">Indicates the ID label.</param>
	/// <param name="Color">Indicates the color.</param>
	[StructLayout(LayoutKind.Explicit)]
	public readonly record struct ColorIdentifier(
		[field: FieldOffset(0)] bool UseId,
		[field: FieldOffset(4)] int Id,
		[field: FieldOffset(4)] int Color
	) : IValueEquatable<ColorIdentifier>
	{
		/// <inheritdoc/>
		public bool Equals(in ColorIdentifier other) => UseId && Id == other.Id || !UseId && Color == other.Color;

		/// <inheritdoc/>
		/// <remarks>
		/// We on purpose show the another property value. For example, if we uses ID
		/// as the instance representing, the result value of get hash code will be the color value,
		/// and vice versa.
		/// </remarks>
		public override int GetHashCode() => UseId ? Color : Id;

		/// <inheritdoc cref="object.ToString"/>
		public override string ToString()
		{
			if (UseId)
			{
				return $"ID = {Id.ToString()}";
			}
			else
			{
				byte a = (byte)(Color >> 24 & 255);
				byte r = (byte)(Color >> 16 & 255);
				byte g = (byte)(Color >> 8 & 255);
				byte b = (byte)(Color & 255);
				return $"Color = #{a.ToString("X2")}{r.ToString("X2")}{g.ToString("X2")}{b.ToString("X2")}";
			}
		}


		/// <summary>
		/// Checks whether two <see cref="ColorIdentifier"/>s contains the same value.
		/// </summary>
		/// <param name="left">The left value to compare.</param>
		/// <param name="right">The right value to compare.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool operator ==(in ColorIdentifier left, in ColorIdentifier right) => left.Equals(right);

		/// <summary>
		/// Checks whether two <see cref="ColorIdentifier"/>s contains the different value.
		/// </summary>
		/// <param name="left">The left value to compare.</param>
		/// <param name="right">The right value to compare.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool operator !=(in ColorIdentifier left, in ColorIdentifier right) => !(left == right);
	}
}
