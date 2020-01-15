using System.Text;

namespace Sudoku.Data.Extensions
{
	public static class StringBuilderEx
	{
		public static StringBuilder Reverse(this StringBuilder @this)
		{
			for (int i = 0, length = @this.Length >> 1; i < length; i++)
			{
				int z = i + 1;
				char temp = @this[i];
				@this[i] = @this[^z];
				@this[^z] = temp;
			}

			return @this;
		}

		public static StringBuilder RemoveFromEnd(this StringBuilder @this, int length) =>
			@this.Remove(@this.Length - length, length);

		public static StringBuilder AppendLine(this StringBuilder @this, char value)
		{
			@this.Append(value);
			@this.AppendLine();

			return @this;
		}

		public static StringBuilder AppendLine(this StringBuilder @this, object? obj) =>
			@this.AppendLine(obj.NullableToString(string.Empty));
	}
}
