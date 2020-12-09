using System;

namespace Sudoku
{
	/// <summary>
	/// Mark on a field of an enumeration type to set a custom name to output.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = false)]
	public sealed class NameAttribute : Attribute
	{
		/// <summary>
		/// Initializes an instance with the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		public NameAttribute(string name) => Name = name;


		/// <summary>
		/// Indicates the name.
		/// </summary>
		public string Name { get; }
	}
}
