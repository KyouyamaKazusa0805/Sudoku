namespace Sudoku.CodeGenerating
{
	/// <summary>
	/// To mark on a type, to tell the user and the compiler that the source generator
	/// should automatically generate the <c>ToString</c> methods
	/// <see cref="object.ToString"/> and <see cref="IFormattable.ToString(string, IFormatProvider)"/>.
	/// </summary>
	/// <seealso cref="object.ToString"/>
	/// <seealso cref="IFormattable.ToString(string, IFormatProvider)"/>
	public sealed class AutoFormattableAttribute : Attribute
	{
	}
}
