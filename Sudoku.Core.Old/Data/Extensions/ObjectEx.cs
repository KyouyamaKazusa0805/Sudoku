namespace Sudoku.Data.Extensions
{
	public static class ObjectEx
	{
		public static string NullableToString(this object? @this) => @this.NullableToString("null");

		public static string NullableToString(this object? @this, string @default) => @this?.ToString() ?? @default;
	}
}
