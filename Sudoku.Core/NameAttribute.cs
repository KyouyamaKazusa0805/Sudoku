using System;
using System.Reflection;

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


		/// <summary>
		/// Get the name of the specified enum field which has marked this attribute.
		/// </summary>
		/// <typeparam name="TEnum">The type of the enum field.</typeparam>
		/// <param name="enumField">The enum field.</param>
		/// <returns>
		/// The name. Return <see langword="null"/> when the specified field doesn't mark this attribute.
		/// </returns>
		public static string? GetName<TEnum>(TEnum enumField) where TEnum : Enum =>
			typeof(TEnum).GetField(enumField.ToString()) is FieldInfo fieldInfo
			&& fieldInfo.GetCustomAttribute<NameAttribute>() is { Name: string result }
			? result
			: null;
	}
}
