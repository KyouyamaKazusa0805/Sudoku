namespace Sudoku.Data.Extensions
{
	public static class ObjectEx
	{
		public static string NullableToString(this object? @this) =>
			@this.NullableToString(string.Empty);

		public static string NullableToString(this object? @this, string defaultValue) =>
			@this?.ToString() ?? defaultValue;
	}
}
